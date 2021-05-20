﻿using AdminPanel.Models;
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
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private VendorModel VendorModel;
        public VendorController(IHttpClientFactory httpClientFactory)
        {
            this.VendorModel = VendorModel.Instance;
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        #region List
        public async Task<IActionResult> Index(int pageNumber, string searchQuery)
        {
            ViewData["vendor"] = "active";
            ViewData["searchString"] = (string.IsNullOrEmpty(searchQuery)) ? "" : searchQuery;
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/vendor?pageSize=15&pageNumber={pageNumber}&searchQuery={searchQuery}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var pagination = response.Headers.GetValues("MVC-Pagination").FirstOrDefault().ToString();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Vendor>>(responseStream);
                var deserializedHeader = JsonSerializer.Deserialize<PaginationHeader>(pagination);
                return View(new VendorIndexViewModel(deserializedResponse.Data, deserializedHeader));
            }
        }
        #endregion

        #region Create
        public IActionResult CreateVendor()
        {
            ViewData["vendor"] = "active";
            return  View(new VendorEditAndCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendor(VendorEditAndCreateViewModel VendorCreate)
        {
            if (!ModelState.IsValid)
            {
                return View(VendorCreate);
            }

            var VendorForCreation = new VendorForCreation()
            {
                Name = VendorCreate.Name,
                Location = VendorCreate.Location,
                SortOrder = VendorCreate.SortOrder,
                CloseAt = VendorCreate.CloseAt,
                OpenAt = VendorCreate.OpenAt,
            };

            // take the first (only) file in the Files list
            var profileImageFile = VendorCreate.ProfileFile.FirstOrDefault();

            try
            {
                if (profileImageFile.Length > 0)
                {
                    using (var fileStream = profileImageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(profileImageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(300, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                VendorForCreation.ProfileBytes = ms.ToArray();
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

            var bannerImageFile = VendorCreate.BannerFile.FirstOrDefault();

            try
            {
                if (bannerImageFile.Length > 0)
                {
                    using (var fileStream = bannerImageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(bannerImageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(900, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                VendorForCreation.BannerBytes = ms.ToArray();
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
            var serializedVendorForCreation = JsonSerializer.Serialize(VendorForCreation);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/vendor");

            request.Content = new StringContent(
                serializedVendorForCreation,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Vendor Created Successfully";
            return RedirectToAction("Index");
        }
        #endregion
        #region Delete
        public async Task<IActionResult> DeleteVendor(Guid id)
        {
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/vendor/{id}");

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
                    TempData["Message"] = "Vendor not found";
                }
                else
                {
                    throw;
                }
                return RedirectToAction("Index");
            }

            TempData["CUD"] = true;
            TempData["Message"] = "Vendor Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("Index");
        }
        #endregion
        #region Edit
        public async Task<IActionResult> EditVendor(Guid id)
        {

            ViewData["vendor"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/vendor/{id}");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var vendor = new Vendor();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Vendor>>(responseStream);
                vendor = deserializedResponse.Data.FirstOrDefault();
                
            }
            request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/category?includeAll=true");

             response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var multiselect = new List<Category>();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Category>>(responseStream);
                 multiselect = deserializedResponse.Data;
            }
            var viewmodel = new VendorEditAndCreateViewModel(vendor,multiselect);
            VendorModel.Categories = multiselect;
            return View(viewmodel);
        }
        [HttpPost]
        public async Task<IActionResult> EditVendor(VendorEditAndCreateViewModel VendorEdit, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var newSelectedCategories = new List<Category>();
            if (VendorModel != null && VendorEdit.SelectedCategories != null)
            {
                foreach (var selectedCategoryId in VendorEdit.SelectedCategories)
                {
                    var selectedCategory = VendorModel.Categories.Where(c => c.Id == selectedCategoryId).FirstOrDefault();
                    newSelectedCategories.Add(selectedCategory);
                }
            }
            
            var editedVendor = new VendorForCreation()
            {
                Name = VendorEdit.Name,
                Location = VendorEdit.Location,
                SortOrder = VendorEdit.SortOrder,
                CloseAt = VendorEdit.CloseAt,
                OpenAt = VendorEdit.OpenAt,
                Categories = newSelectedCategories
            };

            var profileImageFile = VendorEdit.ProfileFile.FirstOrDefault();
            var imageChanged = false;
            try
            {
                if (profileImageFile.Length > 0)
                {
                    imageChanged = true;
                    using (var fileStream = profileImageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(profileImageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(300, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                editedVendor.ProfileBytes = ms.ToArray();
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

            var bannerImageFile = VendorEdit.BannerFile.FirstOrDefault();
            try
            {
                if (bannerImageFile.Length > 0)
                {
                    imageChanged = true;
                    using (var fileStream = bannerImageFile.OpenReadStream())
                    {
                        using (var image = Image.Load(bannerImageFile.OpenReadStream()))
                        {
                            image.Mutate(h => h.Resize(900, 300));
                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsJpeg(ms);
                                editedVendor.BannerBytes = ms.ToArray();
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
            var serializedVendorForEdit = JsonSerializer.Serialize(editedVendor);

             var httpClient = httpClientFactory.CreateClient("APIClient");

             var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/vendor/{id}?imageChanged={imageChanged}");

             request.Content = new StringContent(
                serializedVendorForEdit,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "Vendor Edited Successfully";
            return RedirectToAction("Index");
        }
        #endregion

    }




}
