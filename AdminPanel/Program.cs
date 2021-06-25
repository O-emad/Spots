using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //var api = CreateApiHostBuilder(args).Build();
            //var idp = CreateIdpHostBuilder(args).Build();
            //CreateHostBuilder(args).Build().Run();
            //await Task.WhenAny(
            //    host.RunAsync(),
            //    api.RunAsync()
            //    //idp.RunAsync()
            //    );
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                   
                    webBuilder
                    //.UseUrls("https://*:443")
                    .UseStartup<Startup>();
                });
        //public static IHostBuilder CreateApiHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder => {
        //            webBuilder
        //                //.UseUrls("https://*:80")
        //                .UseStartup<Spots.APIs.Startup>();
        //        });
        //public static IHostBuilder CreateIdpHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder => {
        //            webBuilder
        //                .UseUrls("https://*:5001")
        //                .UseStartup<ExtraSW.IDP.Startup>();
        //        });
    }
}
