using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class Table : IParagraphElement
    {
        #region Properties
        public TableCell[][] TableCells { get; set; }

        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string HeaderText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public Table(TableCell[][] tableCells, Item parentItem)
        {
            TableCells = tableCells;
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
