using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Spots.Domain;
using Spots.Services.Helpers;
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
    public class AdController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AdController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        #region Index
        public async Task<IActionResult> Index(int pageNumber, string searchQuery)
        {
            ViewData["ad"] = "active";
            ViewData["searchString"] = (string.IsNullOrEmpty(searchQuery)) ? "" : searchQuery;
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/ad?pageSize=15&pageNumber={pageNumber}&searchQuery={searchQuery}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var pagination = response.Headers.GetValues("MVC-Pagination").FirstOrDefault().ToString();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Ad>>(responseStream);
                var deserializedHeader = JsonSerializer.Deserialize<PaginationHeader>(pagination);
                return View(new AdIndexViewModel(deserializedResponse.Data, deserializedHeader));
            }

        }
        #endregion


        #region Create
        public IActionResult CreateAd()
        {
            ViewData["ad"] = "active";
            return View(new AdEditAndCreateViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateAd(AdEditAndCreateViewModel createAdVM)
        {
            ViewData["ad"] = "active";
            if (!ModelState.IsValid)
            {
                return View(createAdVM);
            }

            var newAd = new AdForCreation()
            {
                Name = createAdVM.Name,
                SortOrder = createAdVM.SortOrder,
                ExternalLink = createAdVM.ExternalLink
            };

            var adImage = createAdVM.Files.FirstOrDefault();

            try
            {
                if (adImage.Length > 0)
                {
                    using (var fileStream = adImage.OpenReadStream())
                    {
                        using (var image = Image.Load(adImage.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(900, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                newAd.File = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!(e.GetType() == typeof(NullReferenceException)))
                    throw;
            }

            var serializedNewAd = JsonSerializer.Serialize(newAd);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/ad");

            request.Content = new StringContent(
                serializedNewAd,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Ad Created Successfully";
            return RedirectToAction("Index");
        }
        #endregion

        #region Edit
        public IActionResult EditAd()
        {
            ViewData["ad"] = "active";
            return View();
        }
        #endregion

        #region Delete
        public async Task<IActionResult> DeleteAd(Guid id)
        {
            ViewData["ad"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/ad/{id}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

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
                    TempData["Message"] = "Ad not found";
                }
                else
                {
                    throw;
                }
                return RedirectToAction("Index");
            }

            TempData["CUD"] = true;
            TempData["Message"] = "Ad Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
