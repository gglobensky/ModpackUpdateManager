using System.Collections.Generic;

namespace ModpackUpdateManager.Models
{
    public class ModFile
    {

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public string? modLoader;
        public string? loaderVersion;
        public string? license;
        public string? issueTrackerURL;
        public string? displayURL;
        public Mods[] mods;

        public class Mods
        {
            public string? modId;
            //public string? authors;
            public string? version;
            public string? displayName;
            public string? description;
        }
    }

    public class ModFileData
    {
        public string displayName { get; private set; }
        public string rawDisplayName { get; private set; }
        public string displayURL { get; private set; }
        public string fileName { get; private set; }
        public string searchableName { get; private set; }

        public ModFileData(string _displayName, string _rawDisplayName, string? _displayURL, string _fileName, string _searchableName)
        {
            displayName = _displayName;
            rawDisplayName = _rawDisplayName;
            displayURL = _displayURL != null ? _displayURL : "";
            fileName = _fileName;
            searchableName = _searchableName;
        }

    }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Dependency
    {
        public string modId { get; set; }
        public bool mandatory { get; set; }
        public string versionRange { get; set; }
        public string ordering { get; set; }
        public string side { get; set; }
    }

    public class Dependencies
    {
        public List<Dependency> dependency { get; set; }
    }

    public class Mod
    {
        public string modId { get; set; }
        public string version { get; set; }
        public string displayName { get; set; }
        public string displayURL { get; set; }
        public string logoFile { get; set; }
        public string credits { get; set; }
        //public string authors { get; set; }
        public string description { get; set; }
    }

    public class ModTomlFile
    {
        public string modLoader { get; set; }
        public string loaderVersion { get; set; }
        public string license { get; set; }
        public string issueTrackerURL { get; set; }
        public List<Mod> mods { get; set; }
        public Dependencies dependencies { get; set; }
    }

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

    public class LogObject
    {
        public LogObject(string name, string reason)
        {
            Name = name;
            Reason = reason;
        }

        public string Name { get; set; }
        public string Reason { get; set; }

    }
    public class URL
    {
        public URL(string address)
        {
            Address = address;
        }

        public string Address { get; set; }
    }
    public class NameIdPair
    {
        public NameIdPair(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { private get; set; }
        public string Id { private get; set; }

    }
}

