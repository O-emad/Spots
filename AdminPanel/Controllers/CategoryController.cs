using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
//using Newtonsoft.Json;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.IO;
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
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel>(responseStream);
                return View(new CategoryIndexViewModel(deserializedResponse.Data));
            }
        }

        public async Task<IActionResult> AddCategory(AddCategoryViewModel addCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var categoryForCreation = new CategoryForCreation()
            { Name = addCategoryViewModel.Name,
              SortOrder = addCategoryViewModel.SortOrder,
              SuperCategoryId = addCategoryViewModel.SuperCategoryId
            };

            // take the first (only) file in the Files list
            var imageFile = addCategoryViewModel.Files.FirstOrDefault();

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
                            categoryForCreation.Bytes = ms.ToArray();
                        }
                    }
                }


                //using (var fileStream = imageFile.OpenReadStream())
                //using (var ms = new MemoryStream())
                //{
                //    fileStream.CopyTo(ms);
                //    categoryForCreation.Bytes = ms.ToArray();
                //}
            }

            // serialize it
            var serializedCategoryForCreation = JsonSerializer.Serialize(categoryForCreation);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/category");

            request.Content = new StringContent(
                serializedCategoryForCreation,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/category/{id}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return RedirectToAction("Index");
        }

        public PartialViewResult CategoryAddPartialView()
        {
            return PartialView("_CategoryAddQuickView");
        }

        [HttpPost]
        public async Task<PartialViewResult> CategoryEditPartialView(Guid id)
        {
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category/{id}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                return PartialView("_CategoryEditQuickView", new CategoryEditViewModel(
                    await JsonSerializer.DeserializeAsync<Category>(responseStream)));
            }
        }

        


    }


    

}
