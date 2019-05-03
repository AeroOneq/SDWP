using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word
{
    public class WordRenderSettings
    {
        public string FontFamily { get; set; }
        public bool AddFooter { get; set; }
        public bool AddHeader { get; set; }
        public bool AddPageNumber { get; set; }
        public string DefaultTextSize { get; set; }
        public string DefaultColor { get; set; }

    }
}
