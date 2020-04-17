using BibliTech.FileCleaner.Core;
using BibliTech.FileCleaner.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class FileCleanerServicesExtensions
    {

        public static IServiceCollection AddCleaner(this IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(CoreUtils.AssemblyFolder)
                .AddJsonFile("appsettings.json", false);
            var config = configBuilder.Build();
            services.AddSingleton<IConfiguration>(config);

            services.AddLogging(logConfig =>
            {
                logConfig.ClearProviders();

                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .CreateLogger();

                logConfig.AddSerilog(logger, true);
            });

            services.AddScoped<ICleanerSettingsService, CleanerSettingsService>();
            services.AddScoped<ICleanerService, CleanerService>();

            return services;
        }

    }

}
