// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using ExtraSW.IDP.DbContexts;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServerHost.Quickstart.UI;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ExtraSW.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //var connectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=db_a707a9_idp;User Id=db_a707a9_idp_admin;Password=msicx611";
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ExtraSwIdpDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews()
                .AddJsonOptions(opts => {
                    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddDbContext<IdentityDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString
                   )
                .EnableSensitiveDataLogging();
            });

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;

            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients);


            services.AddScoped<ILocalUserService, LocalUserService>();

            builder.AddProfileService<LocalUserProfileService>();
            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
            var migrationAssembly = typeof(Startup)
                .GetTypeInfo().Assembly.GetName().Name;
            builder.AddOperationalStore(o =>
            {
                o.ConfigureDbContext = builder =>
                builder.UseSqlServer(connectionString,
                option => option.MigrationsAssembly(migrationAssembly));
            });
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(o =>
            //    {
            //        o.ApiName = "idpapicollection";
            //        o.Authority = "https://localhost:5001/";
            //    });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            InitializeDatabase(app);
            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }



        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            }
        }
    }
}
