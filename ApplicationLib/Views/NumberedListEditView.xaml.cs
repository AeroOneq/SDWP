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
using ApplicationLib.Interfaces;

namespace ApplicationLib.Views
{
    public partial class NumberedListEditView : UserControl, IParagraphEditView
    {
        private NumberedList NumberedList { get; }
        private HintControl HintControl { get; }
        private ParagraphElementSettings ParagraphSettings { get; set; }
        private ListBox NumberedListListBox { get; }

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        #endregion

        #region Constructors
        public NumberedListEditView(NumberedList numberedList)
        {
            InitializeComponent();

            HintControl = hintControl;
            NumberedList = numberedList;
            ParagraphSettings = paragraphsSettings;
            NumberedListListBox = numberedListListBox;

            DataContext = NumberedList;
            SetParagraphSettingsEvents();
        }
        #endregion

        private void SetParagraphSettingsEvents()
        {
            IParagraphSettings pSettings = ParagraphSettings as IParagraphSettings;

            pSettings.OnParagraphDelete += DeleteParagraph;
            pSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
            pSettings.OnParagraphReplace += ReplaceParagraph;
        }
        #region IParagraphEditView
        public void DeleteParagraph()
        {
            (NumberedList as IParagraphElement).RemoveParagraphFromParentList();
            RefreshParagraphsUI();
        }

        public void ReplaceParagraph()
        {

        }

        public void ShowOrHideHint()
        {
            if (HintControl.Visibility == Visibility.Collapsed)
            {
                HintControl.Visibility = Visibility.Visible;
            }
            else
            {
                HintControl.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region List element event handlers
        private void MoveItemUpper(object sender, MouseButtonEventArgs e)
        {
            NumberedList.MoveItemUp(GetClickedImageIndex(sender));
            RefreshDataContext();
        }

        private int GetClickedImageIndex(object sender)
        {
            Image clickedImage = sender as Image;
            return int.Parse(clickedImage.Uid);
        }

        private void MoveItemLower(object sender, MouseButtonEventArgs e)
        {
            NumberedList.MoveItemDown(GetClickedImageIndex(sender));
            RefreshDataContext();
        }

        private void AddNewItem(object sender, MouseButtonEventArgs e)
        {
            NumberedList.AddItem(GetClickedImageIndex(sender));
            RefreshDataContext();
        }

        private void DeleteItem(object sender, MouseButtonEventArgs e)
        {
            NumberedList.DeleteItem(GetClickedImageIndex(sender));
            RefreshDataContext();
        }

        private void IconMouseEnter(object sender, MouseEventArgs e)
        {
            Image thisImage = sender as Image;
            thisImage.Visibility = Visibility.Collapsed;

            List<Image> images = (thisImage.Parent as Grid).Children.OfType<Image>().ToList();
            int thisImageIndex = images.FindIndex(img => img.Name == thisImage.Name);
            images[thisImageIndex + 1].Visibility = Visibility.Visible;
        }

        private void IconMouseLeave(object sender, MouseEventArgs e)
        {
            Image thisImage = sender as Image;
            thisImage.Visibility = Visibility.Collapsed;

            List<Image> images = (thisImage.Parent as Grid).Children.OfType<Image>().ToList();
            int thisImageIndex = images.FindIndex(img => img.Name == thisImage.Name);
            images[thisImageIndex - 1].Visibility = Visibility.Visible;
        }
        #endregion

        private void RefreshDataContext()
        {
            DataContext = null;
            DataContext = NumberedList;
        }
    }
}
