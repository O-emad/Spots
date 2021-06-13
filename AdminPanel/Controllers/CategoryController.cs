using AdminPanel.Models;
using AdminPanel.Models.Category;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        #region List
        public async Task<IActionResult> Index(int pageNumber, string searchQuery)
        {
            ViewData["category"] = "active";
            ViewData["searchString"] = (string.IsNullOrEmpty(searchQuery)) ? "" : searchQuery;
            if(pageNumber < 1)
            {
                pageNumber = 1;
            }
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category?pageSize=15&pageNumber={pageNumber}&searchQuery={searchQuery}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var pagination = response.Headers.GetValues("MVC-Pagination").FirstOrDefault().ToString();
            
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<CategoryListModel>>(responseStream);
                var deserializedHeader = JsonSerializer.Deserialize<PaginationHeader>(pagination);
                return View(new CategoryIndexViewModel(deserializedResponse.Data,deserializedHeader));
            }
        }
        #endregion

        #region Create
        public async Task<IActionResult> CreateCategory()
        {
            ViewData["category"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category?includeAll=true");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<CategoryListModel>>(responseStream);
                var viewmodel = new CategoryEditAndCreateViewModel();
                foreach (var category in deserializedResponse.Data)
                {
                    var selectItem = new SelectListItem { Text = category.Name, Value = category.Id.ToString() };
                    viewmodel.Categories.Add(selectItem);
                }
                return View(viewmodel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryEditAndCreateViewModel categoryCreate)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var categoryForCreation = new CategoryForCreation()
            {
                NameAR = categoryCreate.NameAR,
                Name = categoryCreate.Name,
                SortOrder = categoryCreate.SortOrder,
                CategoryId = categoryCreate.CategoryId
            };

            // take the first (only) file in the Files list
            var imageFile = categoryCreate.Files.FirstOrDefault();

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
                                categoryForCreation.Bytes = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if(e.GetType() == typeof(UnknownImageFormatException))
                {
                    TempData["Type"] = "alert-danger";
                    TempData["CUD"] = true;
                    TempData["Message"] = "Action Failed : Bad Image Format";
                    return RedirectToAction("index");
                }
                if (!(e.GetType() == typeof(NullReferenceException)))
                    throw;
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
            TempData["Type"] = "alert-success";
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

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                TempData["Type"] = "alert-danger";
                if (e.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["CUD"] = true;
                    TempData["Message"] = "Cannot delete a parent category, Delete children first";
                }
                else if(e.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["CUD"] = true;
                    TempData["Message"] = "Category not found";
                }
                else
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            
            TempData["CUD"] = true;
            TempData["Message"] = "Category Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("Index");
        }
        #endregion
        #region Edit
        public async Task<IActionResult> EditCategory(Guid id)
        {
            ViewData["category"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category?includeAll=true");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var deserializedResponse = await JsonSerializer
                .DeserializeAsync<DeserializedResponseModel<CategoryListModel>>(responseStream);
            var selectList = deserializedResponse.Data;
            var categoryToBeEdited = deserializedResponse.Data.Where(c => c.Id == id).FirstOrDefault();
            selectList.Remove(categoryToBeEdited);

             request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category/{id}");
            request.Headers.Add("X-Lang", "all");

            response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var responseStream2 = await response.Content.ReadAsStreamAsync();
            var deserializedResponse2 = await JsonSerializer
                .DeserializeAsync<DeserializedResponseModel<Category>>(responseStream2);
            var categorySingle = deserializedResponse2.Data.FirstOrDefault();
            var viewmodel = new CategoryEditAndCreateViewModel(categorySingle);

            foreach (var category in selectList)
            {
                var selectItem = new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString()
                };
                viewmodel.Categories.Add(selectItem);
            }
            return View(viewmodel);
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
                NameAR = categoryEdit.NameAR,
                Name = categoryEdit.Name,
                SortOrder = categoryEdit.SortOrder,
                CategoryId = categoryEdit.CategoryId
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
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Category Edited Successfully";
            return RedirectToAction("Index");
        }
        #endregion
    }




}
