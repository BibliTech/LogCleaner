using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibliTech.FileCleaner.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BibliTech.FileCleaner.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Environment.CurrentDirectory = CoreUtils.AssemblyFolder;

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddCleaner();
                    services.AddHostedService<Worker>();
                })
                .UseWindowsService();
        }
    }
}
