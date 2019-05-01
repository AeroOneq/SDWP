using System;
using System.Collections.Generic;
using AeroORMFramework;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Documentation
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("authorID")]
        public int AuthorID { get; set; }
        [JsonProperty("authorName")]
        public string AuthorName { get; set; }
        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [JsonProperty("access")]
        public Access Access { get; set; }
        [JsonProperty("storageType")]
        public StorageType StorageType { get; set; }
    }
}
