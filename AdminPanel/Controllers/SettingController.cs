using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    //[Authorize
    public class SettingController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public SettingController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }
        public async Task<IActionResult> Index()
        {
            ViewData["setting"] = "active";

            var httpClient = clientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/setting/");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using (var streamReader = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<Setting>>(streamReader);
                return View(new SettingViewModel(deserializedResponse.Data.FirstOrDefault()));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting(SettingViewModel newSetting)
        {
            ViewData["setting"] = "active";
            if (!ModelState.IsValid)
            {
                return View(newSetting);
            }
            var setting = new SettingForCreation();
            {
                setting.AutomaticOfferApproval = newSetting.Settings.AutomaticOfferApproval;
            }
            var serializedSetting = JsonSerializer.Serialize(setting);
            var httpClient = clientFactory.CreateClient("APIClient");
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                "/api/setting");
            request.Content = new StringContent(
                serializedSetting,
                Encoding.Unicode,
                "application/json");
            var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Setting Edited Successfully";
            return RedirectToAction("Index");
        }

    }
}
