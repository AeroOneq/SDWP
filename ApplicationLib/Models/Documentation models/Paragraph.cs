using System.Collections.Generic;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class Paragraph
    {
        public List<IParagraphElement> ParagraphElements { get; set; }
    }
}
  