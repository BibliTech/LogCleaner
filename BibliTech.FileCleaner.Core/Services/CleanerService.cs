using BibliTech.FileCleaner.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BibliTech.FileCleaner.Core.Services
{
    public interface ICleanerService
    {
        Task<int> CleanAsync();
    }

    internal class CleanerService : ICleanerService
    {
        const string LongFileNamePrefix = @"\\?\";

        readonly ICleanerSettingsService settingsService;
        readonly ILogger<CleanerService> logger;
        public CleanerService(ICleanerSettingsService settingsService, ILogger<CleanerService> logger)
        {
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public async Task<int> CleanAsync()
        {
            int result = 60;
            try
            {
                this.logger.LogInformation($"Entering {nameof(this.CleanAsync)} with account ${CoreUtils.RunningAccount.Value}.");

                var settings = this.settingsService.Settings;
                result = settings.Interval;

                var tasks = new List<Task>();
                foreach (var item in settings.Items)
                {
                    tasks.Add(Task.Run(() => this.Clean(item)));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }

            return result;
        }

        void Clean(CleanerItem item)
        {
            if (string.IsNullOrEmpty(item.Folder))
            {
                this.logger.LogInformation($"Skipping Empty Folder item.");
                return;
            }

            if (item.Lifetime == int.MaxValue)
            {
                this.logger.LogInformation($"Skipping {item.Folder} because Lifetime is not set.");
                return;
            }

            this.logger.LogInformation($"Cleaning {item.Folder}.");

            var now = DateTime.UtcNow;
            var minModTime = now.AddSeconds(-item.Lifetime);

            var folder = new DirectoryInfo(item.Folder);
            try
            {
                this.ScanFolder(folder, minModTime, item.DeleteEmptySubfolders, true);
            }
            catch (IOException)
            {
                try
                {
                    this.SetOwner(folder);
                    this.ScanFolder(folder, minModTime, item.DeleteEmptySubfolders, true);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, ex.Message);
                }
            }
        }

        void ScanFolder(DirectoryInfo folder, DateTime minModTime, bool deleteEmptyFolder, bool isTop)
        {
            try
            {
                var folderName = folder.FullName;
                if (folderName.Length > 255 && !folderName.StartsWith(""))
                {
                    folderName = LongFileNamePrefix + folderName;
                    folder = new DirectoryInfo(folderName);
                }

                this.logger.LogDebug($"Scanning {folder.FullName}");

                foreach (var file in folder.EnumerateFiles())
                {
                    var usingFile = file;
                    var fileName = usingFile.FullName;
                    if (fileName.Length > 255 && !fileName.StartsWith(LongFileNamePrefix))
                    {
                        fileName = LongFileNamePrefix + fileName;
                        usingFile = new FileInfo(fileName);
                    }

                    if (file.LastWriteTimeUtc < minModTime)
                    {
                        this.logger.LogDebug($"Deleting {usingFile.FullName}");
                        this.Delete(usingFile);
                    }
                }

                foreach (var subFolder in folder.EnumerateDirectories())
                {
                    this.ScanFolder(subFolder, minModTime, deleteEmptyFolder, false);
                }

                if (deleteEmptyFolder && !isTop && !folder.EnumerateFileSystemInfos().Any())
                {
                    this.logger.LogDebug($"Empty Folder. Deleting {folder.FullName}");
                    this.Delete(folder);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }            
        }

        void SetOwner(DirectoryInfo folder)
        {
            var acl = folder.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);

            acl.SetOwner(CoreUtils.RunningAccount);
            acl.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                CoreUtils.RunningUser, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            folder.SetAccessControl(acl);
        }

        void SetOwner(FileInfo file)
        {
            // Remove long filename from ACL: the library already handles that
            if (file.FullName.StartsWith(LongFileNamePrefix))
            {
                file = new FileInfo(file.FullName.Substring(LongFileNamePrefix.Length));
            }

            var acl = file.GetAccessControl(System.Security.AccessControl.AccessControlSections.All);

            acl.SetOwner(CoreUtils.RunningAccount);
            acl.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(
                CoreUtils.RunningUser, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

            file.SetAccessControl(acl);
        }

        void Delete(FileInfo file)
        {
            try
            {
                file.Delete();
            }
            catch (Exception)
            {
                try
                {
                    this.SetOwner(file);
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, file.FullName);
                }
            }
        }

        void Delete(DirectoryInfo folder)
        {
            try
            {
                folder.Delete();
            }
            catch (Exception)
            {
                try
                {
                    this.SetOwner(folder);
                    folder.Attributes = FileAttributes.Normal;
                    folder.Delete();
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, folder.FullName);
                }
            }
        }

    }

}
