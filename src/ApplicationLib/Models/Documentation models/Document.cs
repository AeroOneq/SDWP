using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Document
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("documentationID")]
        public int DocumentationID { get; set; }
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

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
}
