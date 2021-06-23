using IdentityServer4;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Text.Json;
using IdentityModel.Client;
using IdentityServer4.Models;
using System.Security.Claims;
using IdentityServer4.Contrib.HttpClientService;
using IdentityModel;
using System.Security.Cryptography;
namespace Spots.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILocalUserService localUserService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory clientFactory;

        public AuthController(ILocalUserService localUserService,IHttpContextAccessor httpContextAccessor,
                              IHttpClientFactory clientFactory)
        {
            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }


        //Client and workflow values
        private const string clientBaseUri = @"http://localhost:44308";
        private const string validIssuer = "SomeSecureCompany";
        private const string response_type = "code";
        private const string grantType = "authorization_code";

        //IdentityServer4
        private const string idPServerBaseUri = "https://idp.rokiba.com";//"https://localhost:5001";//////
        private const string idPServerAuthUri = idPServerBaseUri + "/connect/authorize";
        private const string idPServerTokenUriFragment = @"connect/token";
        private const string idPServerEndSessionUri = idPServerBaseUri + @"/connect/endsession";

        //These are also registered in the IdP (or Clients.cs of test IdP)
        private const string redirectUri = "https://api.rokiba.com";
        private const string clientId = "adminpanelclient";
        private const string clientSecret = "secret";
        private const string audience = "SomeSecureCompany/resources";
        private const string scope = "categoryapi roles openid";


        //Store values using cookie-based authentication middleware
        private void SaveState(string state)
        {
            var tempId = new ClaimsIdentity("TempCookie");
            tempId.AddClaim(new Claim("state", state));

            //this.Request.GetOwinContext().Authentication.SignIn(tempId);
        }





        [HttpPost]
        public async Task<IActionResult> GoogleAuthenticate([FromQuery] string id_token)
        {
            
            Payload payload = await ValidateAsync(id_token, new ValidationSettings
            {
                Audience = new[] { Startup.StaticConfig["Authentication:Google:ClientId"] }
            });
            var user = await localUserService
                .GetOrCreateExternalLoginUser("Google", payload.Subject, payload.Email, payload.GivenName, payload.FamilyName);
            var httpClient = clientFactory.CreateClient("IDPClient");

            var state = Guid.NewGuid().ToString("N");

            var url = "/connect/authorize" +
                        "?client_id=" + clientId +
                        "&response_type=" + response_type +
                        "&redirect_uri=" + redirectUri +
                        "&scope=" + scope +
                        "&response_mode=" + "form_post"+
                        "&state=" + state;
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                url
                );
            var response1 = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            

            var serializeduser = JsonSerializer.Serialize(user);

            request = new HttpRequestMessage(
                HttpMethod.Post,
                $"/api/externallogin");

            request.Content = new StringContent(
                serializeduser,
                System.Text.Encoding.Unicode,
                "application/json");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

             request = new HttpRequestMessage(
                HttpMethod.Get,
                url
                );
             response1 = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);


            

            var responseStream = await response1.Content.ReadAsStringAsync();
            var dd = responseStream.IndexOf("value");
            var bb = responseStream.IndexOf('\n');
            var sfd = responseStream.Substring(dd, bb - dd);
            var sds = sfd.Split('\'');
            var authorizationCode = sds[1];

            var client = new HttpClient();
            var responseToken = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
            {
                Address = idPServerBaseUri + "/connect/token",
                ClientId = "adminpanelclient",
                ClientSecret = "secret",//new Secret("secret".Sha256()).ToString(),
                Code = authorizationCode,
                GrantType = "authorization_code",
                RedirectUri = redirectUri
            }).ConfigureAwait(false);
            var responseModel = new ResponseModel();
            //if (!string.IsNullOrWhiteSpace(responseToken.AccessToken))
            //{
            //    responseModel.StatusCode = StatusCodes.Status404NotFound;
            //    responseModel.Message = "No Access Token Generated";
            //    return NotFound(responseModel);
            //}
            responseModel.StatusCode = StatusCodes.Status200OK;
            responseModel.Message = "Success";
            responseModel.Data = new { accessToken = responseToken.AccessToken };
            return Ok(responseModel);
        }

    }
}
