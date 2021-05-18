using AdminPanel.Models;
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

        public VendorController(IHttpClientFactory httpClientFactory)
        {
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

       // #region Create
        public IActionResult CreateVendor()
        {
            ViewData["vendor"] = "active";
            //var httpClient = httpClientFactory.CreateClient("APIClient");

            //var request = new HttpRequestMessage(
            //    HttpMethod.Get,
            //    $"/api/category/");

            //var response = await httpClient.SendAsync(
            //    request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            //response.EnsureSuccessStatusCode();

            //using (var responseStream = await response.Content.ReadAsStreamAsync())
            //{
            //    var deserializedResponse = await JsonSerializer
            //        .DeserializeAsync<DeserializedResponseModel<Vendor>>(responseStream);
            //    var viewmodel = new VendorEditAndCreateViewModel();
            //    foreach (var Vendor in deserializedResponse.Data)
            //    {
            //        var selectItem = new SelectListItem { Text = Vendor.Name, Value = Vendor.Id.ToString() };
            //        viewmodel.Categories.Add(selectItem);
            //    }
            //    return View(viewmodel);
            //}
            return  View(new VendorEditAndCreateViewModel());
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateVendor(VendorEditAndCreateViewModel VendorCreate)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // create an ImageForCreation instance
        //    var VendorForCreation = new VendorForCreation()
        //    {
        //        NameAR = VendorCreate.NameAR,
        //        Name = VendorCreate.Name,
        //        SortOrder = VendorCreate.SortOrder,
        //        SuperVendorId = VendorCreate.SuperVendorId
        //    };

        //    // take the first (only) file in the Files list
        //    var imageFile = VendorCreate.Files.FirstOrDefault();

        //    try
        //    {
        //        if (imageFile.Length > 0)
        //        {
        //            using (var fileStream = imageFile.OpenReadStream())
        //            {
        //                using (var image = Image.Load(imageFile.OpenReadStream()))
        //                {
        //                    image.Mutate(h => h.Resize(300, 300));
        //                    using (var ms = new MemoryStream())
        //                    {
        //                        image.SaveAsJpeg(ms);
        //                        VendorForCreation.Bytes = ms.ToArray();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (!(e.GetType() == typeof(NullReferenceException)))
        //            throw;
        //    }

        //    // serialize it
        //    var serializedVendorForCreation = JsonSerializer.Serialize(VendorForCreation);

        //    var httpClient = httpClientFactory.CreateClient("APIClient");

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Post,
        //        $"/api/Vendor");

        //    request.Content = new StringContent(
        //        serializedVendorForCreation,
        //        System.Text.Encoding.Unicode,
        //        "application/json");

        //    var response = await httpClient.SendAsync(
        //        request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();
        //    TempData["Type"] = "alert-success";
        //    TempData["CUD"] = true;
        //    TempData["Message"] = "Vendor Created Successfully";
        //    return RedirectToAction("Index");
        //}
        //#endregion
        //#region Delete
        //public async Task<IActionResult> DeleteVendor(Guid id)
        //{
        //    var httpClient = httpClientFactory.CreateClient("APIClient");

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Delete,
        //        $"/api/Vendor/{id}");

        //    var response = await httpClient.SendAsync(
        //        request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        //    try
        //    {
        //        response.EnsureSuccessStatusCode();
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        TempData["Type"] = "alert-danger";
        //        if (e.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            TempData["CUD"] = true;
        //            TempData["Message"] = "Cannot delete a parent Vendor, Delete children first";
        //        }
        //        else if (e.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            TempData["CUD"] = true;
        //            TempData["Message"] = "Vendor not found";
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //        return RedirectToAction("Index");
        //    }

        //    TempData["CUD"] = true;
        //    TempData["Message"] = "Vendor Deleted Successfully";
        //    TempData["Type"] = "alert-success";
        //    return RedirectToAction("Index");
        //}
        //#endregion
        //#region Edit
        //public async Task<IActionResult> EditVendor(Guid id)
        //{

        //    ViewData["Vendor"] = "active";
        //    var httpClient = httpClientFactory.CreateClient("APIClient");

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Get,
        //        $"/api/Vendor/");

        //    var response = await httpClient.SendAsync(
        //        request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();

        //    using (var responseStream = await response.Content.ReadAsStreamAsync())
        //    {
        //        var deserializedResponse = await JsonSerializer
        //            .DeserializeAsync<DeserializedResponseModel>(responseStream);
        //        var VendorToBeEdited = deserializedResponse.Data.Where(c => c.Id == id).FirstOrDefault();
        //        var viewmodel = new VendorEditAndCreateViewModel(VendorToBeEdited);
        //        var selectList = deserializedResponse.Data.ToList();
        //        selectList.Remove(VendorToBeEdited);
        //        foreach (var Vendor in selectList)
        //        {
        //            var selectItem = new SelectListItem { Text = Vendor.Name, Value = Vendor.Id.ToString() };
        //            viewmodel.Categories.Add(selectItem);
        //        }
        //        return View(viewmodel);
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> EditVendor(VendorEditAndCreateViewModel VendorEdit, Guid id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    var editedVendor = new VendorForCreation()
        //    {
        //        NameAR = VendorEdit.NameAR,
        //        Name = VendorEdit.Name,
        //        SortOrder = VendorEdit.SortOrder,
        //        SuperVendorId = VendorEdit.SuperVendorId
        //    };

        //    // take the first (only) file in the Files list
        //    var imageFile = VendorEdit.Files.FirstOrDefault();
        //    var imageChanged = false;
        //    try
        //    {
        //        if (imageFile.Length > 0)
        //        {
        //            imageChanged = true;
        //            using (var fileStream = imageFile.OpenReadStream())
        //            {
        //                using (var image = Image.Load(imageFile.OpenReadStream()))
        //                {
        //                    image.Mutate(h => h.Resize(300, 300));
        //                    using (var ms = new MemoryStream())
        //                    {
        //                        image.SaveAsJpeg(ms);
        //                        editedVendor.Bytes = ms.ToArray();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (!(e.GetType() == typeof(NullReferenceException)))
        //            throw;
        //    }


        //    // serialize it
        //    var serializedVendorForEdit = JsonSerializer.Serialize(editedVendor);

        //    var httpClient = httpClientFactory.CreateClient("APIClient");

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Put,
        //        $"/api/Vendor/{id}?imageChanged={imageChanged}");

        //    request.Content = new StringContent(
        //        serializedVendorForEdit,
        //        System.Text.Encoding.Unicode,
        //        "application/json");

        //    var response = await httpClient.SendAsync(
        //        request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        //    response.EnsureSuccessStatusCode();
        //    TempData["Type"] = "alert-success";
        //    TempData["CUD"] = true;
        //    TempData["Message"] = "Vendor Edited Successfully";
        //    return RedirectToAction("Index");
        //}
        //#endregion

    }




}
