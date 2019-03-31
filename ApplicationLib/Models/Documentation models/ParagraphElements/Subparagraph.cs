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

namespace ApplicationLib.Models
{
    public class Subparagraph : IParagraphElement
    {
        #region Properties
        public string Text { get; set; }

        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string HeaderText { get; set; }
        #endregion

        public Subparagraph() { }
        public Subparagraph(string text, Item parentItem)
        {
            Text = text;
            ParentItem = parentItem;
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }

        public UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public UserControl GetEditView() => new SubparagraphEditView(this);
    }
}
