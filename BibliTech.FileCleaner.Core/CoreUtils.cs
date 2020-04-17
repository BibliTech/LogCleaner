using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BibliTech.FileCleaner.Core
{

    public static class CoreUtils
    {

        public static string AssemblyFolder { get; } =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    }

}
