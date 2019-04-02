using System.Collections.Generic;
using ApplicationLib.Interfaces;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Item
    {
        public string Name { get; set; }

        public List<Item> Items { get; set; }
        public List<IParagraphElement> Paragraphs { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; set; }
        [JsonIgnore]
        public List<Item> ParentList { get; set; }
    }
}
