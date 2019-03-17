using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class Table : IParagraphElement
    {
        public TableCell[][] TableCells { get; set; }
        public Paragraph ParentParagraph { get; }

        public Table(TableCell[][] tableCells, Paragraph parentParagraph)
        {
            TableCells = tableCells;
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
