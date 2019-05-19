using System;
using System.Collections.Generic;
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
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }
        [JsonProperty("teamLeadName")]
        public string TeamLeadName { get; set; }
        [JsonProperty("managerName")]
        public string ManagerName { get; set; }
        [JsonProperty("projectCode")]
        public string ProjectCode { get; set; }
        [JsonProperty("softwareEngineerName")]
        public string SoftwareEngineerName { get; set; }
    }
}
