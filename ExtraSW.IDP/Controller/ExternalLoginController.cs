using ExtraSW.IDP.Entities;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraSW.IDP.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalLoginController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IClientStore clientStore;
        private readonly IAuthenticationSchemeProvider schemeProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILocalUserService localUserService;
        private readonly IEventService events;

        public ExternalLoginController(IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IHttpContextAccessor httpContextAccessor, ILocalUserService localUserService)
        {
            this.interaction = interaction;
            this.clientStore = clientStore;
            this.schemeProvider = schemeProvider;
            this.events = events;
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin([FromBody] User user)
        {
            var isuser = new IdentityServerUser(user.Subject)
            {
                DisplayName = user.UserName
            };
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(new TimeSpan(7, 0, 0, 0, 0))
            };

            await HttpContext.SignInAsync(isuser, props);
            return Ok();
        }

    }
}
