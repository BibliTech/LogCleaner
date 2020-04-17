using BibliTech.FileCleaner.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace BibliTech.FileCleaner.Core.Services
{
    public interface ISettingsService
    {
        CleanerSettings Settings { get; }
    }

    internal class SettingsService : ISettingsService
    {
        const string AppSettingsFileName = "appsettings.json";

        static readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        static CleanerSettings settings;
        readonly ILogger<SettingsService> logger;
        CleanerSettings instanceSettings;
        public SettingsService(ILogger<SettingsService> logger)
        {
            this.logger = logger;
        }

        public CleanerSettings Settings
        {
            get
            {
                locker.Wait();
                try
                {
                    if (this.instanceSettings == null)
                    {
                        if (settings == null)
                        {

                            this.logger.LogInformation("Loading settings.");

                            var json = File.ReadAllText(Path.Combine(
                                CoreUtils.AssemblyFolder, AppSettingsFileName));
                            settings = JsonSerializer.Deserialize<CleanerSettings>(json);
                        }
                        else
                        {
                            this.logger.LogInformation("Using old settings.");
                        }

                        this.instanceSettings = settings;
                    }
                }
                finally
                {
                    locker.Release();
                }

                return this.instanceSettings;
            }
        }

    }

}
