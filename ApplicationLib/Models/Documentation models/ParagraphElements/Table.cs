using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ApplicationLib.Interfaces;
using ApplicationLib.Views;

using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Table : IParagraphElement
    {
        #region Properties
        public string[][] TableCells { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string Title { get; set; }
        #endregion

        public Table(string[][] tableCells, Item parentItem)
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
            return new TableEditView(this);
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }
    }
}
