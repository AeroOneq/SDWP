using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using ApplicationLib.Models;

namespace SDWP.Models
{
    public class DocTreeViewItem : TreeViewItem, INotifyPropertyChanged
    {
        public Item Item { get; set; }

        private string headerText;
        public string HeaderText
        {
            get
            {
                return headerText;
            }
            set
            {
                headerText = value;
                OnPropertyChanged("HeaderText");
            }
        }

        private bool isEnabledForEdditing;
        public bool IsEnabledForEdditing
        {
            get
            {
                return isEnabledForEdditing;
            }
            set
            {
                isEnabledForEdditing = value;
                OnPropertyChanged("IsEnabledForEdditing");
            }
        }

        public DocTreeViewItem(Item item) : base()
        {
            Item = item;

            HeaderText = Item.Name;

            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (prop == "HeaderText")
            {
                Item.Name = HeaderText;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
