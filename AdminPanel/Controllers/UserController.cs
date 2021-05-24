using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Spots.Services.Helpers;
//using Spots.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    
    public class UserController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IActionResult> Index(int pageNumber, string searchQuery)
        {
            ViewData["user"] = "active";
            ViewData["searchString"] = (string.IsNullOrEmpty(searchQuery)) ? "" : searchQuery;
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/usermanage?pageSize=15&pageNumber={pageNumber}&searchQuery={searchQuery}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var pagination = response.Headers.GetValues("MVC-Pagination").FirstOrDefault().ToString();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<UserModel>>(responseStream);
                var deserializedHeader = JsonSerializer.Deserialize<PaginationHeader>(pagination);
                return View(new UserIndexViewModel(deserializedResponse.Data, deserializedHeader));
            }
        }

        public IActionResult EditUser()
        {
            return View();
        }

        public IActionResult CreateUser()
        {
            return View();
        }

    }
}
