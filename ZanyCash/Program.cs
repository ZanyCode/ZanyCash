using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZanyCash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile("streamNames.json", optional: false, reloadOnChange: false);
               })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseUrls("https://0.0.0.0:5001", "http://0.0.0.0:5000");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
