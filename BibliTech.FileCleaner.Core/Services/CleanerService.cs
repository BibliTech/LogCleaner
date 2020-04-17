using BibliTech.FileCleaner.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliTech.FileCleaner.Core.Services
{
    public interface ICleanerService
    {
        Task CleanAsync();
    }

    internal class CleanerService : ICleanerService
    {
        readonly ISettingsService settingsService;
        readonly ILogger<CleanerService> logger;
        public CleanerService(ISettingsService settingsService, ILogger<CleanerService> logger)
        {
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public async Task CleanAsync()
        {
            this.logger.LogInformation($"Entering {nameof(this.CleanAsync)}.");

            var settings = this.settingsService.Settings;

            var tasks = new List<Task>();
            foreach (var item in settings.Items)
            {
                tasks.Add(Task.Run(() => this.Clean(item)));
            }

            await Task.WhenAll(tasks);
        }

        void Clean(CleanerItem item)
        {
            this.logger.LogInformation($"Cleaning {item.Folder}.");

            var now = DateTime.UtcNow;
            var minModTime = now.AddSeconds(-item.Lifetime);

            var folder = new DirectoryInfo(item.Folder);
            this.ScanFolder(folder, minModTime, item.DeleteEmptySubfolders, true);
        }

        void ScanFolder(DirectoryInfo folder, DateTime minModTime, bool deleteEmptyFolder, bool isTop)
        {
            this.logger.LogDebug($"Scanning {folder.FullName}");

            foreach (var file in folder.EnumerateFiles())
            {
                if (file.LastWriteTimeUtc < minModTime)
                {
                    this.logger.LogDebug($"Deleting {file.FullName}");
                    file.Delete();
                }
            }

            foreach (var subFolder in folder.EnumerateDirectories())
            {
                this.ScanFolder(subFolder, minModTime, deleteEmptyFolder, false);
            }

            if (deleteEmptyFolder && !isTop && !folder.EnumerateFileSystemInfos().Any())
            {
                this.logger.LogDebug($"Empty Folder. Deleting {folder.FullName}");
                folder.Delete();
            }
        }

    }

}
