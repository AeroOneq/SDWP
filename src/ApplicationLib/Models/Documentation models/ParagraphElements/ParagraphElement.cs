using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class ParagraphElement : IParagraphElement
    {
        [JsonProperty("hint")]
        public string Hint { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonIgnore]
        public Paragraph ParentParagraph { get; set; }

        public virtual UserControl GetEditView()
        {
            return null;
        }
    }
}
