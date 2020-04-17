using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BibliTech.FileCleaner.Test.Core
{


    public class TestHelper
    {
        const int MaxSeconds = 8640000; // 100 days
        const int MinFile = 0;
        const int MaxFile = 10;
        const int MinFolder = 0;
        const int MaxFolder = 3;
        const int TotalFolderCount = 1000;

        int remainingFolder;

        [Fact]
        public async Task CreateRootTestFolder()
        {
            var now = DateTime.UtcNow;
            var random = new Random();

            Directory.Delete(TestUtils.TestFolder, true);
            await Task.Delay(500);

            this.remainingFolder = TotalFolderCount;

            while (this.remainingFolder > 0)
            {
                this.CreateTestFolder(TestUtils.TestFolder, now, random);
            }
        }

        void CreateTestFolder(string folder, DateTime now, Random random)
        {
            Directory.CreateDirectory(folder);
            this.remainingFolder--;

            var fileCount = random.Next(MinFile, MaxFile);
            for (int i = 0; i < fileCount; i++)
            {
                var fileName = Path.GetRandomFileName();
                var filePath = Path.Combine(folder, fileName);
                using (File.Create(filePath)) { }

                var lifeTime = random.Next(MaxSeconds);
                File.SetLastWriteTimeUtc(filePath, now.AddSeconds(-lifeTime));
            }

            var shouldHaveFolder = random.Next(3) == 0;
            if (!shouldHaveFolder)
            {
                return;
            }

            var folderCount = random.Next(MinFolder, MaxFolder);
            for (int i = 0; i < folderCount; i++)
            {
                var folderName = Path.GetRandomFileName();
                var folderPath = Path.Combine(folder, folderName);
                Directory.CreateDirectory(folderPath);

                var lifeTime = random.Next(MaxSeconds);
                Directory.SetLastWriteTimeUtc(folderPath, now.AddSeconds(-lifeTime));

                this.CreateTestFolder(folderPath, now, random);
            }
        }
    }

}
