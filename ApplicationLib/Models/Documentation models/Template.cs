using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;

using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Template
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("userID")]
        public int UserID { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
