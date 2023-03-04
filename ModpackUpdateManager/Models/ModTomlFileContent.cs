namespace ModpackUpdateManager.Models
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public class ModTomlFileContent
    {

        public string? modLoader;
        public string? loaderVersion;
        public string? license;
        public string? issueTrackerURL;
        public string? displayURL;
        public Mods[] mods;

        public class Mods
        {
            public string? modId;
            public string? version;
            public string? displayName;
            public string? description;
        }
    }

#pragma warning restore CS8632 

}

