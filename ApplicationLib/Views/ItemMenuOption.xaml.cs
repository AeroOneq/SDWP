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
        #endregion

        #region Events
        /// <summary>
        /// This event is called when some changes were perfomed which must then refresh
        /// the list in UI where this item is placed (e.g. deleteion of the item)
        /// </summary>
        public Action UpdateList { get; set; }
        /// <summary>
        /// This event is called when the item's button is called
        /// </summary>
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
            UpdateList();
        }

        private void MoveItemUp(object sender, RoutedEventArgs e)
        {
            if (Item.ParentList.FindIndex(i => i.Equals(Item)) == 0)
            {
                Item.ParentList.Remove(Item);
                Item.ParentList.Add(Item);
            }
            else
            {
                int itemIndex = Item.ParentList.FindIndex(i => i.Equals(Item));

                Item temp = Item.ParentList[itemIndex - 1];
                Item.ParentList[itemIndex - 1] = Item;
                Item.ParentList[itemIndex] = temp;
            }

            UpdateList();
        }

        private void MoveItemDown(object sender, RoutedEventArgs e)
        {
            if (Item.ParentList.FindIndex(i => i.Equals(Item)) == Item.ParentList.Count - 1)
            {
                for (int i = Item.ParentList.Count - 1; i > 0; i--)
                {
                    Item.ParentList[i] = Item.ParentList[i - 1];
                }

                Item.ParentList[0] = Item;
            }
            else
            {
                int itemIndex = Item.ParentList.FindIndex(i => i.Equals(Item));

                Item temp = Item.ParentList[itemIndex + 1];
                Item.ParentList[itemIndex + 1] = Item;
                Item.ParentList[itemIndex] = temp;
            }

            UpdateList();
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
