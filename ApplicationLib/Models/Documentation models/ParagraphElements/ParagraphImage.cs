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
    public class ParagraphImage : IParagraphElement, IParentableParagraph
    {
        #region Properties
        public byte[] ImageSource { get; set; }
        public string Hint { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; private set; }
        [JsonIgnore]
        public List<IParagraphElement> ParentList { get; private set; }
        #endregion

        public ParagraphImage(byte[] imageSource)
        {
            ImageSource = imageSource;
        }

        public void SetParents(Item parentItem, List<IParagraphElement> parentList)
        {
            ParentItem = parentItem;
            ParentList = parentList;
        }

        public UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public UserControl GetEditView()
        {
            return new ImageEditView(this);
        }

        public void RemoveParagraphFromParentList()
        {
            ParentList.Remove(this);
        }
    }
}
