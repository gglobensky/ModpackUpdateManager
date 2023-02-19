using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModpackUpdateManager.Models
{
    public class ModData
    {
        public string displayName { get; private set; }
        public string rawDisplayName { get; private set; }
        public string displayURL { get; private set; }
        public string fileName { get; private set; }
        public string searchableName { get; private set; }

        public ModData(string _displayName, string _rawDisplayName, string? _displayURL, string _fileName, string _searchableName)
        {
            displayName = _displayName;
            rawDisplayName = _rawDisplayName;
            displayURL = _displayURL != null ? _displayURL : "";
            fileName = _fileName;
            searchableName = _searchableName;
        }

    }
}
