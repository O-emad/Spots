using AdminPanel.HttpHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Microsoft.AspNetCore.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using System.Text.Json.Serialization;

namespace AdminPanel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(opts =>
                {
                });

            //services.AddAuthorization(o =>
            //{
            //    o.AddPolicy("CanCUDCategory",
            //        policyBuilder =>
            //        {
            //            policyBuilder.RequireAuthenticatedUser();
            //            policyBuilder.RequireClaim("level", "Admin");
            //        });
            //});

            services.AddHttpContextAccessor();
            services.AddTransient<BearerTokenHandler>();
            services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44308");
                //client.BaseAddress = new Uri("https://api.rokiba.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<BearerTokenHandler>();

            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                //client.BaseAddress = new Uri("https://idp.rokiba.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json");
            });


            //services.AddHttpClient("IDPAPIClient", client =>
            //{
            //    // client.BaseAddress = new Uri("https://localhost:5001");
            //    client.BaseAddress = new Uri("https://idp.rokiba.com");
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json");
            //}).AddHttpMessageHandler<BearerTokenHandler>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o=>
            {
                o.AccessDeniedPath = ("/Authorization/accessDenied");
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
            {
                //o.RequireHttpsMetadata = false;
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.Authority = "https://localhost:5001";
                //o.Authority = "https://idp.rokiba.com";
                o.ClientId = "adminpanelclient";
                o.ResponseType = "code";
                //o.Scope.Add("openid");
                //o.Scope.Add("profile");
                o.Scope.Add("roles");
                o.Scope.Add("categoryapi");
                o.Scope.Add("idpapi");
                //o.UsePkce = true;
                //o.ClaimActions.Remove("nbf");
                o.ClaimActions.MapUniqueJsonKey("role", "role");
                //o.ClaimActions.DeleteClaim("sid");
                //o.ClaimActions.DeleteClaim("idp");
                //o.ClaimActions.DeleteClaim("s_hash");
                //o.ClaimActions.DeleteClaim("auth_time");
                o.SaveTokens = true;
                o.ClientSecret = "secret";
                o.GetClaimsFromUserInfoEndpoint = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.GivenName,
                    RoleClaimType = JwtClaimTypes.Role
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
