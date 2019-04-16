using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SDWP.Models
{
    public class TemplateTreeViewItem : TreeViewItem, INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
