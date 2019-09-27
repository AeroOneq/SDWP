using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Models
{
    public class NumberedListElement
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("index")]
        public string Index { get; set; }

        public NumberedListElement() { }
        public NumberedListElement(string text)
        {
            Text = text;
        }
    }
}