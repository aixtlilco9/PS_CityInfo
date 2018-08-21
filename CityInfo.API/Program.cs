using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //originally had the line below as the only code here but modified it so db could be seeded as recomended by ms 
            //BuildWebHost(args).Run();
            var host = BuildWebHost(args);
            using ( var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<CityInfoContext>();
                    CityInfoExtensions.EnsureSeedDataForContext(context);
                }
                catch(Exception ex)
                {
                    //not sure what this logs too but most liekly would not be the logger i implmented and not sure if i could log to it from here
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            //added this belwo for the logging also check this out if ever needed https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
            .ConfigureLogging(logging =>
            {
                logging.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            })
            //added the above
                .Build();
    }
}
