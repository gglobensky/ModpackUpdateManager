using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Models
{
    public class ModSearchResult
    {
        public ModSearchResult(string value, string url, string updated)
        {
            Value = value;
            Url = url;
            Updated = updated;
        }

        public string Value { get; set; }
        public string Url { get; set; }
        public string Updated { get; set; }
    }
}
