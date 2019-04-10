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
    public class ParagraphElement : IParagraphElement, IParentableParagraph
    {
        public string Hint { get; set; }
        public string Title { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; private set; }
        [JsonIgnore]
        public List<ParagraphElement> ParentList { get; private set; }

        public void SetParents(Item parentItem, List<ParagraphElement> parentList)
        {
            ParentItem = parentItem;
            ParentList = parentList;
        }

        public void RemoveParagraphFromParentList()
        {
            ParentList.Remove(this);
        }

        public virtual UserControl GetWatchView()
        {
            return null;
        }
        public virtual UserControl GetEditView()
        {
            return null;
        }
    }
}
