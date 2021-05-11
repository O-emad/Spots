using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
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
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IActionResult> Index()
        {

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/api/category/");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                return View(new CategoryIndexViewModel(
                    await JsonSerializer.DeserializeAsync<List<Category>>(responseStream)));
            }
        }
    }


    

}
