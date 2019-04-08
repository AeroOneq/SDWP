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
    public class Subparagraph : IParagraphElement, IParentableParagraph
    {
        #region Properties
        public string Text { get; set; }
        public string Hint { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; private set; }
        [JsonIgnore]
        public List<IParagraphElement> ParentList { get; private set; }
        #endregion

        public Subparagraph(string text)
        {
            Text = text;
        }

        public void SetParents(Item parentItem, List<IParagraphElement> parentList)
        {
            ParentItem = parentItem;
            ParentList = parentList;
        }

        public void RemoveParagraphFromParentList()
        {
            ParentList.Remove(this);
        }

        public UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public UserControl GetEditView() => new SubparagraphEditView(this);
    }
}
