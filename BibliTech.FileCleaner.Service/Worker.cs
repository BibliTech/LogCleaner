using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BibliTech.FileCleaner.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BibliTech.FileCleaner.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        IServiceProvider services;
        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            this.logger = logger;
            this.services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("Worker running.");

                    using (var scope = this.services.CreateScope())
                    {
                        var cleaner = scope.ServiceProvider.GetRequiredService<ICleanerService>();
                        var delay = await cleaner.CleanAsync();

                        await Task.Delay(delay * 1000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.logger.LogError(ex, "Error while cleaning");
                    }
                    catch (Exception) { }
                }
            }
        }
    }
}
