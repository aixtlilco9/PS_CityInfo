using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
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
