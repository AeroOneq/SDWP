using System.Collections.Generic;
using ApplicationLib.Interfaces;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Item : IParentableItem
    {
        public string Name { get; set; }

        public List<Item> Items { get; set; }
        public List<Paragraph> Paragraphs { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; private set; }
        [JsonIgnore]
        public List<Item> ParentList { get; private set; }

        public void SetParents(Item parentItem, List<Item> parentList)
        {
            ParentItem = parentItem;
            ParentList = parentList;
        }
    }
}
