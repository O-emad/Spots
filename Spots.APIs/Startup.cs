using ExtraSW.IDP.DbContexts;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using Spots.Data;
using Spots.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Spots.APIs
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            StaticConfig = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services)
        {
            //var apiConnectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=db_a707a9_api;User Id=db_a707a9_api_admin;Password=msicx611";
            //var idpConnectionString = "Data Source=SQL5097.site4now.net;Initial Catalog=db_a707a9_idp;User Id=db_a707a9_idp_admin;Password=msicx611";
            var apiConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SpotsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var idpConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ExtraSwIdpDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            services.AddControllers(opt =>
            {
                opt.ReturnHttpNotAcceptable = true;
            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                setupAction.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new DefaultNamingStrategy()
                };
            });
            //.AddJsonOptions(opts => {
            //    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
            //    //opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //} );

            services.AddCors(opt =>
            {
                opt.AddPolicy(MyAllowSpecificOrigins, builer =>
                 {
                     builer.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
                 });
            });

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

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("SpotsOpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Spots API",
                        Version = "1"
                    });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });

            services.AddHttpContextAccessor();

            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
                //client.BaseAddress = new Uri("https://idp.rokiba.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(Microsoft.Net.Http.Headers.HeaderNames.Accept, "application/json");
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
            app.UseCors(MyAllowSpecificOrigins);
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/SpotsOpenAPISpecification/swagger.json",
                    "Spots API");
                setupAction.RoutePrefix = "";
            });
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
