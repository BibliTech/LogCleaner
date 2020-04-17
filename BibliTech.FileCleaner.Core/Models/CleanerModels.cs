using System;
using System.Collections.Generic;
using System.Text;

namespace BibliTech.FileCleaner.Core.Models
{
    
    public class CleanerSettings
    {

        public IEnumerable<CleanerItem> Items { get; set; }
        public int Interval { get; set; }
        public bool ReadSettingsEveryRun { get; set; }

    }

    public class CleanerItem
    {
        public string Folder { get; set; }
        public int Lifetime { get; set; }
        public bool DeleteEmptySubfolders { get; set; }
    }

}
