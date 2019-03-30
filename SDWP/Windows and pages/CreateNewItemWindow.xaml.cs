using ApplicationLib.Models;
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
using System.Windows.Shapes;

using ApplicationLib.Interfaces;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для CreateNewItemWindow.xaml
    /// </summary>
    public partial class CreateNewItemWindow : Window
    {
        private List<Item> CurrentItemsList { get; set; }
        private Item CurrentItem { get; set; }

        #region Constructors
        public CreateNewItemWindow() { InitializeComponent(); }
        public CreateNewItemWindow(List<Item> items, Item currentItem)
        {
            InitializeComponent();

            CurrentItemsList = items;
            CurrentItem = currentItem;
        }
        #endregion

        /// <summary>
        /// Creates a new item. If the content creation mode is selected then item object
        /// is initialized with the null in Items and empty list in Paragraphs, if the items creation
        /// mode is selected then everything is conversed.
        /// </summary>
        private void CreateNewItem(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(itemNameTextBox.Text))
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Введите имя нового пункта", MessageBoxButton.OK);
                return;
            }

            if (contentCreationModeRadioBtn.IsChecked == true)
            {
                CurrentItemsList.Add(new Item()
                {
                    Items = null,
                    Paragraphs = new List<IParagraphElement>(),
                    ParentItem = CurrentItem,
                    ParentList = CurrentItemsList,
                    Name = itemNameTextBox.Text
                });
            }
            else
            {
                CurrentItemsList.Add(new Item()
                {
                    Items = new List<Item>(),
                    Paragraphs = null,
                    ParentItem = CurrentItem,
                    ParentList = CurrentItemsList,
                    Name = itemNameTextBox.Text
                });
            }

            Close();
        }

        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
