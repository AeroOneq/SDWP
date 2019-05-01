using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApplicationLib.Interfaces;

using Newtonsoft.Json;

using ApplicationLib.Views;

namespace ApplicationLib.Models
{
    public class ParagraphImage : ParagraphElement
    {
        #region Properties
        [JsonProperty("source")]
        public byte[] ImageSource { get; set; }
        #endregion

        public ParagraphImage() { }
        public ParagraphImage(byte[] imageSource)
        {
            ImageSource = imageSource;
        }

        public override UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public override UserControl GetEditView()
        {
            return new ImageEditView(ParentParagraph);
        }
    }
}
