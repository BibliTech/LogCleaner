using BibliTech.FileCleaner.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BibliTech.FileCleaner.Test.Core.Services
{

    public class CleanerServiceTest : IDisposable
    {

        static IServiceProvider services = TestUtils.ConfigureServices();

        IServiceScope scope;
        ICleanerService cleaner;
        public CleanerServiceTest()
        {
            this.scope = services.CreateScope();
            this.cleaner = services.GetRequiredService<ICleanerService>();
        }

        public void Dispose()
        {
            this.scope.Dispose();
        }

        [Fact]
        public async Task TestCleaner()
        {
            await this.cleaner.CleanAsync();
        }
    }

}
