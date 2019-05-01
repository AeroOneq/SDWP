using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Subparagraph : ParagraphElement
    {
        #region Properties
        [JsonProperty("text")]
        public string Text { get; set; }
        #endregion

        public Subparagraph() { }
        public Subparagraph(string text)
        {
            Text = text;
        }

        public override UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public override UserControl GetEditView() => new SubparagraphEditView(ParentParagraph);
    }
}
