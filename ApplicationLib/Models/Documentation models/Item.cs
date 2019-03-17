using System.Collections.Generic;

namespace ApplicationLib.Models
{
    public class Item
    {
        public List<Item> Items { get; set; }
        
        public List<Paragraph> Paragraphs { get; set; }
    }
}
