using AdminPanel.Models;
using AdminPanel.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Spots.Services.Helpers;
//using Spots.Services.Helpers;
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
    
    public class UserController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IActionResult> Index(int pageNumber, string searchQuery)
        {
            if (User.IsInRole("Vendor"))
            {
                return RedirectToAction("EditUser", Guid.Empty);
            }
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

        public async Task<IActionResult> EditUser(Guid id)
        {
            ViewData["user"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"/api/usermanage/{id}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer.
                    DeserializeAsync<DeserializedResponseModel<UserModel>>(responseStream);
                return View(new UserEditViewModel(deserializedResponse.Data.FirstOrDefault()));
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel vm,  Guid id)
        {
            ViewData["user"] = "active";
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = new UserForEdit()
            {
                Password = vm.Password,
                Active = vm.Active
            };
            //user.Claims.Add(new UserClaimForCreation()
            //{
            //    Type = "role",
            //    Value = vm.Role
            //});
            if (!string.IsNullOrWhiteSpace(vm.GivenName))
            {
                user.Claims.Add(new UserClaimForCreation()
                {
                    Type = "given_name",
                    Value = vm.GivenName
                });
            }
            if (!string.IsNullOrWhiteSpace(vm.FamilyName))
            {
                user.Claims.Add(new UserClaimForCreation()
                {
                    Type = "family_name",
                    Value = vm.FamilyName
                });
            }
            var serializedEditedUser = JsonSerializer.Serialize(user);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"/api/usermanage/{id}");

            request.Content = new StringContent(
                serializedEditedUser,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                 TempData["Type"] = "alert-danger";
                if (e.StatusCode == HttpStatusCode.Conflict || e.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["CUD"] = true;
                    TempData["Message"] = "Bad user format";
                    return View(vm);
                    
                }
                else
                {
                    throw;
                }
            }
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "User Edited Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult CreateUser()
        {
            ViewData["user"] = "active";
            return View(new UserCreateViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateViewModel vm)
        {
            ViewData["user"] = "active";
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var userToCreate = new UserForCreation()
            {
                Password = vm.Password,
                UserName = vm.Username,
                Subject = Guid.NewGuid().ToString(),
                Active = true
            };
            userToCreate.Claims.Add(new UserClaimForCreation()
            {
                Type = "role",
                Value = vm.Role
            });
            if (!string.IsNullOrWhiteSpace(vm.GivenName))
            {
                userToCreate.Claims.Add(new UserClaimForCreation()
                {
                    Type = "given_name",
                    Value = vm.GivenName
                });
            }
            if (!string.IsNullOrWhiteSpace(vm.FamilyName))
            {
                userToCreate.Claims.Add(new UserClaimForCreation()
                {
                    Type = "family_name",
                    Value = vm.FamilyName
                });
            }
            var serializedNewUser = JsonSerializer.Serialize(userToCreate);

            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/usermanage");

            request.Content = new StringContent(
                serializedNewUser,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
               // TempData["Type"] = "alert-danger";
                if (e.StatusCode == HttpStatusCode.Conflict || e.StatusCode == HttpStatusCode.BadRequest)
                {
                    return View(vm);
                    //TempData["CUD"] = true;
                    //TempData["Message"] = "Ad not found";
                }
                else
                {
                    throw;
                }
            }
            TempData["Type"] = "alert-success";
            TempData["CUD"] = true;
            TempData["Message"] = "User Created Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ViewData["user"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"/api/usermanage/{id}");

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
                    TempData["Message"] = e.Message;
                }
                else
                {
                    throw;
                }
                return RedirectToAction("Index");
            }

            TempData["CUD"] = true;
            TempData["Message"] = "User Deleted Successfully";
            TempData["Type"] = "alert-success";
            return RedirectToAction("Index");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

    }
}
