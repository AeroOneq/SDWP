using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using ApplicationLib.Models;

namespace SDWP.Models
{
    public class TemplateTreeViewItemItem : TemplateTreeViewItem
    {
        public Item Item { get; set; }

        public TemplateTreeViewItemItem(Item item) : base()
        {
            Item = item;
            HeaderText = Item.Name;

            DataContext = this;
        }

        public override void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (prop == "HeaderText")
                Item.Name = HeaderText;

            base.OnPropertyChanged(prop);
        }
    }
}
