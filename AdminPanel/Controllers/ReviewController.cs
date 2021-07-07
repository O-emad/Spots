using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class ReviewController :Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private VendorModel VendorModel;
        public ReviewController(IHttpClientFactory httpClientFactory)
        {
            this.VendorModel = VendorModel.Instance;
            this.httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
            
        }

        public async Task<IActionResult> Index(Guid id)
        {
            if (User.IsInRole("Vendor"))
            {
                var vendorId = await VendorModel.GetVendorId(httpClientFactory);
                if (vendorId == null)
                {
                    TempData["Type"] = "alert-danger";
                    TempData["CUD"] = true;
                    TempData["Message"] = "No vendor is linked to this user";
                    return RedirectToAction("index", "Dashboard");
                }
                ViewData["review"] = "active";
                if (vendorId.Value != id)
                {
                    return RedirectToAction("Index", new { id = vendorId });
                }
            }
            else
            {
                ViewData["vendor"] = "active";
            }
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/vendor/{id}/review");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<Review>>(responseStream);
                return View(new ReviewIndexViewModel(deserializedResponse.Data));
            }
        }

        public async Task<IActionResult> GetSingleReview(Guid id, Guid vendorId)
        {
            ViewData["vendor"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/vendor/{vendorId}/review/{id}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<Review>>(responseStream);
                return View();
            }
        }

        public async Task<IActionResult> DeleteReview(Guid id, Guid vendorId)
        {
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/vendor/{vendorId}/review/{id}");

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
                    TempData["Message"] = "Review or Vendor not found";
                }
                else
                {
                    throw;
                }
                return RedirectToAction("Index", new { id = vendorId });
            }
            TempData["CUD"] = true;
            TempData["Message"] = "Review Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("Index", new { id = vendorId });
        }

    }
}
