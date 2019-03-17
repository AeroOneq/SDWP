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

        public Paragraph ParentParagraph { get; }

        public NumberedList(List<NumberedListElement> listElements, Paragraph parentParagraph)
        {
            ListElements = listElements;
            ParentParagraph = parentParagraph;
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
