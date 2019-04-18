using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AeroORMFramework;

using ApplicationLib.Interfaces;

using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Template
    {
        [PrimaryKey]
        [AutoincrementID]
        [CanBeNull(false)]
        public int ID { get; set; }
        [CanBeNull(false)]
        public int UserID { get; set; }

        [CanBeNull(false)]
        public string TemplateName { get; set; }
        [CanBeNull(false)]
        public DateTime CreationAt { get; set; }
        [CanBeNull(false)]
        public DateTime UpdatedAt { get; set; }

        [CanBeNull(false)]
        [Json]
        public List<Item> Items { get; set; }

        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
