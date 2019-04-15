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

using SDWP.Interfaces;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для DocumentTemplatesPage.xaml
    /// </summary>
    public partial class DocumentTemplatesPage : Page, IAccountPage
    {
        #region IAccountPage
        public Action CloseAccGrid { get; set; }
        #endregion

        #region Propeties
        private UserInfo CurrentUser { get; }

        private TextBlock SelectedTemplateTextBlock { get; set; }
        #endregion
        public DocumentTemplatesPage(UserInfo currentUser)
        {
            InitializeComponent();

            CurrentUser = currentUser;
        }

        #region Event handlers
        private void ListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            selectedItem.IsSelected = true;
            SelectedTemplateTextBlock.Text = (selectedItem.DataContext as Documentation).Name;
        }

        private void ListBoxItemMouseEnter(object sender, MouseEventArgs e)
        {
            (sender as ListBoxItem).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        }

        private void ListBoxItemMouseLeave(object sender, MouseEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            if (!listBoxItem.IsSelected)
            {
                listBoxItem.Background = new SolidColorBrush(Colors.LightGray);
            }
        }
        #endregion
    }
}
