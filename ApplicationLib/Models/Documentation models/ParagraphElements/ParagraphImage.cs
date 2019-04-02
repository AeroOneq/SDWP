using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApplicationLib.Interfaces;

using Newtonsoft.Json;


namespace ApplicationLib.Models
{
    public class ParagraphImage : IParagraphElement
    {
        #region Properties
        public byte[] ImageSource { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public ParagraphImage(byte[] imageSource, Item parentItem)
        {
            ImageSource = imageSource;
            ParentItem = parentItem;
        }

        public UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public UserControl GetEditView()
        {
            throw new NotImplementedException();
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }
    }
}
