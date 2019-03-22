using System.Collections.Generic;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class Item
    {
        public string Name { get; set; }
        public List<Item> Items { get; set; }
        
        public List<IParagraphElement> Paragraphs { get; set; }
    }
}
