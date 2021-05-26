using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Spots.Domain;
using Spots.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public OfferController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<IActionResult> ListOffer(Guid id)
        {
            ViewData["vendor"] = "active";
            var httpClient = clientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/vendor/{id}?includeOffer=true");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<Vendor>>(responseStream);
                return View(new OfferIndexViewModel(deserializedResponse.Data.FirstOrDefault()));
            }
        }

        public async Task<IActionResult> GetSingleOffer(Guid id, Guid vendorId, bool toDash = false)
        {

                TempData["todash"] = toDash;

            ViewData["vendor"] = "active";
            var httpClient = clientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/vendor/{vendorId}/offer/{id}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using(var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<Offer>>(responseStream);
                return View(new SingleOfferViewModel(deserializedResponse.Data.FirstOrDefault()));
            }
        }

        public IActionResult CreateOffer(Guid id, string vendorName)
        {
            ViewData["vendor"] = "active";
            return View(new OfferCreateViewModel(id,vendorName));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffer(Guid id, OfferCreateViewModel offerCreate)
        {
            if (!ModelState.IsValid)
            {
                return View(offerCreate);
            }

            var offerForCreation = new OfferForCreationDto();
            offerForCreation.Title = offerCreate.Offer.Title;
            offerForCreation.Description = offerCreate.Offer.Description;
            offerForCreation.Value = offerCreate.Offer.Value;
            offerForCreation.Type = (offerCreate.Offer.Type == "1") ? OfferValueType.Percentage : OfferValueType.Value;

            var imageFile = offerCreate.Files.FirstOrDefault();

            try
            {
                if (imageFile.Length > 0)
                {
                    using (var fileStream = imageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(imageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(300, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                offerForCreation.Bytes = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(UnknownImageFormatException))
                {
                    TempData["Type"] = "alert-danger";
                    TempData["CUD"] = true;
                    TempData["Message"] = "Action Failed : Bad Image Format";
                    return RedirectToAction("index");
                }
                if (!(e.GetType() == typeof(NullReferenceException)))
                    throw;
            }
            var httpClient = clientFactory.CreateClient("APIClient");

            var serializedOfferForCreation = JsonSerializer.Serialize(offerForCreation);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/vendor/{id}/offer/");

            request.Content = new StringContent(
                serializedOfferForCreation,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Offer Created Successfully";
            return RedirectToAction("ListOffer", "Offer",new { id });

        }
        

        public async Task<IActionResult> DeleteOffer(Guid id, Guid vendorId)
        {
            var httpClient = clientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/vendor/{vendorId}/offer/{id}");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                TempData["Type"] = "alert-danger";
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["CUD"] = true;
                    TempData["Message"] = "Offer or Vendor not found";
                }
                else
                {
                    throw;
                }
                return RedirectToAction("ListOffer", "Offer", new { id = vendorId });
            }
            TempData["CUD"] = true;
            TempData["Message"] = "Offer Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("ListOffer", "Offer", new { id = vendorId });
        }

        public async Task<IActionResult> AcceptOffer(Guid id, Guid vendorId)
        {
            var httpClient = clientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/pendingoffer/{id}");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            TempData["CUD"] = true;
            TempData["Message"] = "Offer Accepted Successfully";
            TempData["Type"] = "alert-success";
            if (TempData.ContainsKey("todash"))
            {
                if ((bool)TempData["todash"] == true)
                    return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("ListOffer", "Offer", new { id = vendorId });

        }

    }
}
