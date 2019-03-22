using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    class NumberedList : IParagraphElement
    {
        public List<NumberedListElement> ListElements { get; set; }

        public Item ParentItem { get; }

        public NumberedList(List<NumberedListElement> listElements, Item parentItem)
        {
            ListElements = listElements;
            ParentItem = parentItem;
        }

        public FrameworkElement VisualizeElement()
        {
            throw new NotImplementedException();
        }

        public FrameworkElement GetWatchView()
        {
            throw new NotImplementedException();
        }

        public FrameworkElement GetEditView()
        {
            throw new NotImplementedException();
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }
    }
}
