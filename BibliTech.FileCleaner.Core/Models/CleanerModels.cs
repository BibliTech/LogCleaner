using System;
using System.Collections.Generic;
using System.Text;

namespace BibliTech.FileCleaner.Core.Models
{
    
    public class CleanerSettings
    {

        public IEnumerable<CleanerItem> Items { get; set; } = new CleanerItem[0];
        public int Interval { get; set; } = 60;
        public bool ReadSettingsEveryRun { get; set; } = true;

    }

    public class CleanerItem
    {
        public string Folder { get; set; }
        public int Lifetime { get; set; } = int.MaxValue;
        public bool DeleteEmptySubfolders { get; set; }
    }

}
