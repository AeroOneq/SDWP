using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ApplicationLib.Models;


namespace ApplicationLib.Views
{
    /// <summary>
    /// Логика взаимодействия для ItemMenuOption.xaml
    /// </summary>
    public partial class ItemMenuOption : UserControl
    {
        #region Properties
        public Button ItemBtn { get; }
        public Item Item { get; }
        #endregion

        #region Constructors
        public ItemMenuOption() { InitializeComponent(); }
        public ItemMenuOption(Item item)
        {
            InitializeComponent();

            Item = item;

            ItemBtn = itemBtn;
            ItemBtn.Content = item.Name;
        }
        #endregion
    }
}
