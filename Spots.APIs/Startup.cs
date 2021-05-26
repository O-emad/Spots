using ExtraSW.IDP.DbContexts;
using IdentityServer4.AccessTokenValidation;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Spots.Data;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spots.APIs
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //var apiConnectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=db_a707a9_api;User Id=db_a707a9_api_admin;Password=msicx611";
            //var idpConnectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=db_a707a9_idp;User Id=db_a707a9_idp_admin;Password=msicx611";
            var apiConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SpotsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var idpConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ExtraSwIdpDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            services.AddControllers(opt =>
            {
                opt.ReturnHttpNotAcceptable = true;
            })
                .AddJsonOptions(opts => {
                    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
                } );
            services.AddDbContext<SpotsContext>(opt =>
            {
                //opt.UseSqlServer(configuration[SpotsConfig.ConnectionStringKey.Replace("__", ":")]
                //   )
                opt.UseSqlServer(apiConnectionString
                   )
                .EnableSensitiveDataLogging();
            });
            services.AddDbContext<IdentityDbContext>(opt =>
            {
                opt.UseSqlServer(idpConnectionString
                   )
                .EnableSensitiveDataLogging();
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ISpotsRepositroy, SpotsRepository>();
            services.AddScoped<ILocalUserService, LocalUserService>();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(o =>
                {
                    o.ApiName = "categoryapicollection";
                    //in development
                     o.Authority = "https://localhost:5001/";
                    //in production
                    //o.Authority = "https://idp.rokiba.com";
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
                app.UseExceptionHandler(appbuilder =>
                {
                    appbuilder.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync("An unexpected fault happened. try again later");
                    });
                });
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseStatusCodePages();
        }
    }
}
