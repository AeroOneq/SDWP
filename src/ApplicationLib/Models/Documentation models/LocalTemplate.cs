using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;

using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class LocalTemplate : ISerializable
    {
        public string FileName { get; set; }
        public Template Template { get; set; }

        public LocalTemplate(Template template)
        {
            Template = template;
        }

        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
