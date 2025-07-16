using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FraudReporting
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
                    LoadApfConfigs(hostingContext, config);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    SetEnvironment(webBuilder);
                    webBuilder.UseStartup<Startup>();
                });

        private static void LoadApfConfigs(HostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;
            config.AddJsonFile($@"ApfConfigs\ConnectionStrings.json", optional: false, reloadOnChange: true);
  
            config.AddJsonFile(@"ApfConfigs\PasswordManager.json", optional: false, reloadOnChange: true);
        }

        private static void SetEnvironment(IWebHostBuilder webBuilder)
        {
            webBuilder
#if (LOCAL)
                .UseEnvironment("Local");
#elif (DEVELOPMENT)
                .UseEnvironment("Development");
#elif (STAGING)
                .UseEnvironment("Staging");
#elif (PRODUCTION)
                .UseEnvironment("Production");
#endif
        }
    }
}
