using System;
using System.Collections.Generic;

namespace ModpackUpdateManager.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public int id { get; set; }
        public string changelogBody { get; set; }
        public int childFileType { get; set; }
        public DateTime dateCreated { get; set; }
        public DateTime dateModified { get; set; }
        public string displayName { get; set; }
        public int fileLength { get; set; }
        public string fileName { get; set; }
        public List<string> gameVersions { get; set; }
        public List<int> gameVersionTypeIds { get; set; }
        public int projectId { get; set; }
        public int releaseType { get; set; }
        public int status { get; set; }
        public int totalDownloads { get; set; }
        public int uploadSource { get; set; }
        public User user { get; set; }
        public bool hasServerPack { get; set; }
        public int additionalFilesCount { get; set; }
    }

    public class Pagination
    {
        public int index { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
    }

    public class ModDataApiResponse
    {
        public List<Data> data { get; set; }
        public Pagination pagination { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public int id { get; set; }
        public string twitchAvatarUrl { get; set; }
    }


}
