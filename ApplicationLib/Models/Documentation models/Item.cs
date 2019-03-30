using System.Collections.Generic;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class Item
    {
        public string Name { get; set; }

        public List<Item> Items { get; set; }
        public Item ParentItem { get; set; }
        public List<Item> ParentList { get; set; }
        
        public List<IParagraphElement> Paragraphs { get; set; }
    }
}
