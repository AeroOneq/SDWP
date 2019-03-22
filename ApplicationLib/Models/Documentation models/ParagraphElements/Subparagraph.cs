using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;

namespace ApplicationLib.Models
{
    public class Subparagraph : IParagraphElement
    {
        public string Text { get; set; }
        public Item ParentItem { get; }

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

        public FrameworkElement GetWatchView() => new SubparagraphWatchView(this);

        public FrameworkElement GetEditView()
        {
            throw new NotImplementedException();
        }
    }
}
