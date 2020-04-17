using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibliTech.FileCleaner.Test.Core
{

    public static class TestUtils
    {
        public const string TestFolder = @"D:\Temp\TestCleaner";

        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddCleaner();
            
            return services.BuildServiceProvider();
        }

    }

}
