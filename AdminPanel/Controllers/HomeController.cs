using AdminPanel.Models;
using AdminPanel.ViewModels;
//using ExtraSW.IDP.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(ILogger<HomeController> logger , IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IActionResult> Index()
        {
            ViewData["home"] = "active";
            var httpClient = httpClientFactory.CreateClient("APIClient");
            var vm = new HomeIndexViewModel();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/vendor?includeAll=true");
            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var vendors = new List<VendorDomainModel>();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<VendorDomainModel>>(responseStream);
                vm.VendorsCount = deserializedResponse.Data.Count;
                vendors = deserializedResponse.Data;
            }
            int follows = 0;
            foreach (var vendor in vendors)
            {
                follows += vendor.Follows;
            }
            vm.FollowsCount = follows;
            if (User.IsInRole("Vendor"))
            {
                return View(vm);
            }
            


             request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/ad?includeAll=true");
             response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Ad>>(responseStream);
                vm.AdsCount = deserializedResponse.Data.Count();
            }

            request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/category?includeAll=true");
            response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Category>>(responseStream);
                vm.CategoriesCount = deserializedResponse.Data.Count();
            }

            

            request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/pendingoffer");
            response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var offers = new List<Offer>();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var deserializedResponse = await JsonSerializer
                    .DeserializeAsync<DeserializedResponseModel<Offer>>(responseStream);
                offers = deserializedResponse.Data;
            }

            var query = from offer in offers
                        join vendor in vendors
                        on offer.VendorId equals vendor.Id
                        select new OfferAcceptanceModel()
                        {
                            OfferId = offer.Id,
                            VendorId = vendor.Id,
                            OfferTitle = offer.Title,
                            VendorName = vendor.Name
                        };

            vm.PendingOffers = query.ToList();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
