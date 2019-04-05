using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Models
{
    public class NumberedListElement
    {
        public string Text { get; set; }

        public NumberedListElement(string text)
        {
            Text = text;
        }
    }
}