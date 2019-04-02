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
    class NumberedList : IParagraphElement
    {
        #region Properties
        public List<NumberedListElement> ListElements { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string Title { get; set; }
        #endregion

        public NumberedList(List<NumberedListElement> listElements, Item parentItem)
        {
            ListElements = listElements;
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
