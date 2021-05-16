using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        #region List
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
        #endregion

        #region Create
        public async Task<IActionResult> AddCategory(AddCategoryViewModel addCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var categoryForCreation = new CategoryForCreation()
            {
                Name = addCategoryViewModel.Name,
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

            TempData["CUD"] = true;
            TempData["Message"] = "Category Created Successfully";
            return RedirectToAction("Index");
        }
        #endregion
        #region Delete
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/category/{id}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            TempData["CUD"] = true;
            TempData["Message"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
        #endregion
        #region Edit
        public async Task<IActionResult> EditCategory(Guid id)
        {
            

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category/");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel>(responseStream);
                var categoryToBeEdited = deserializedResponse.Data.Where(c => c.Id == id).FirstOrDefault();
                var viewmodel = new CategoryEditAndCreateViewModel(categoryToBeEdited);
                deserializedResponse.Data.ToList().Remove(categoryToBeEdited);
                foreach (var category in deserializedResponse.Data)
                {
                    var selectItem = new SelectListItem { Text = category.Name, Value = category.Id.ToString() };
                    viewmodel.Categories.Add(selectItem);
                }
                return View(viewmodel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryEditAndCreateViewModel categoryEdit, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var editedCategory = new CategoryForCreation()
            {
                Name = categoryEdit.Name,
                SortOrder = categoryEdit.SortOrder,
                SuperCategoryId = categoryEdit.SuperCategoryId
            };

            // take the first (only) file in the Files list
            var imageFile = categoryEdit.Files.FirstOrDefault();
            var imageChanged = false;
            try
            {
                if (imageFile.Length > 0)
                {
                    imageChanged = true;
                    using (var fileStream = imageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(imageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(300, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                editedCategory.Bytes = ms.ToArray();
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
            

            // serialize it
            var serializedCategoryForEdit = JsonSerializer.Serialize(editedCategory);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/category/{id}?imageChanged={imageChanged}");

            request.Content = new StringContent(
                serializedCategoryForEdit,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            TempData["CUD"] = true;
            TempData["Message"] = "Category Edited Successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }




}
