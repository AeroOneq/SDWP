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
    public partial class ItemMenuOption : UserControl
    {
        #region Properties
        public Button ItemBtn { get; }
        public Item Item { get; }
        private TextBox ItemBtnTextBox { get; set; }
        private Image ItemTypeImage { get; set; }
        public EventHandler OnItemClick { get; set; }
        #endregion

        #region Constructors
        public ItemMenuOption() { InitializeComponent(); }
        public ItemMenuOption(Item item)
        {
            InitializeComponent();

            Item = item;

            ItemBtn = itemBtn;

            if (item.Paragraphs == null)
            {
                itemListTypeImage.Visibility = Visibility.Visible;
            }
            else
            {
                itemContentTypeImage.Visibility = Visibility.Visible;
            }

            ItemBtnTextBox = itemNameTextBox;
            ItemBtnTextBox.Text = Item.Name;
            ItemBtnTextBox.ContextMenu = ContextMenu;
        }
        #endregion

        private void StartRenamingItem(object sender, RoutedEventArgs e)
        {
            MakeItemBtnTextBoxNotReadOnly(ItemBtnTextBox);
        }

        private void MakeItemBtnTextBoxNotReadOnly(TextBox textBox)
        {
            textBox.Cursor = Cursors.IBeam;
            textBox.IsReadOnly = false;
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Item.ParentList.Remove(Item);
        }

        private void OnItemBtnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                MakeItemBtnTextBoxReadOnly(ItemBtnTextBox);
                Item.Name = textBox.Text;
            }
        }

        private void OnItemBtnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ItemBtnTextBox.Text = Item.Name;
            MakeItemBtnTextBoxReadOnly(ItemBtnTextBox);
        }

        private void MakeItemBtnTextBoxReadOnly(TextBox textBox)
        {
            textBox.IsReadOnly = true;
            textBox.Cursor = Cursors.Arrow;
        }

        private void OnItemBtnTextBoxClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemBtnTextBox.IsReadOnly)
            {
                OnItemClick(this, e);
            }
        }

        private void OnItemBtnTextBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartRenamingItem(null, null);
        }
    }
}
