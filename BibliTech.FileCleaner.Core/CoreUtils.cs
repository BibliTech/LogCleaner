using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace BibliTech.FileCleaner.Core
{

    public static class CoreUtils
    {

        public static string AssemblyFolder { get; } = AppDomain.CurrentDomain.BaseDirectory;

        public static string RunningUser { get; } = $"{Environment.UserDomainName}\\{Environment.UserName}";
        public static NTAccount RunningAccount { get; } = new NTAccount(Environment.UserDomainName, Environment.UserName);

    }

}
