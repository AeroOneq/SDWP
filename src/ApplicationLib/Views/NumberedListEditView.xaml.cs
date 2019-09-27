using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        public Paragraph Paragraph { get; }
        private HintControl HintControl { get; }
        private ParagraphElementSettings ParagraphSettings { get; set; }

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        public Action RefreshParagraphsUIAfterSwap { get; set; }
        #endregion

        #region Constructors
        public NumberedListEditView(Paragraph paragraph)
        {
            InitializeComponent();

            Paragraph = paragraph;
            NumberedList = paragraph.ParagraphElement as NumberedList;

            HintControl = hintControl;
            ParagraphSettings = paragraphsSettings;

            DataContext = NumberedList;
            SetParagraphSettingsEvents();
        }
        #endregion

        private void SetParagraphSettingsEvents()
        {
            IParagraphSettings pSettings = ParagraphSettings as IParagraphSettings;

            pSettings.OnParagraphDelete += DeleteParagraph;
            pSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
            pSettings.MoveParagraphDown = MoveParagraphDown;
            pSettings.MoveParagraphUp = MoveParagraphUp;
        }
        #region IParagraphEditView
        public void DeleteParagraph()
        {
            (Paragraph as IParentableParagraph).RemoveParagraphFromParentList();
            RefreshParagraphsUI();
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

        public void MoveParagraphUp()
        {
            if (Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph)) == 0)
            {
                Paragraph.ParentList.Remove(Paragraph);
                Paragraph.ParentList.Add(Paragraph);
            }
            else
            {
                int itemIndex = Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph));

                Paragraph temp = Paragraph.ParentList[itemIndex - 1];
                Paragraph.ParentList[itemIndex - 1] = Paragraph;
                Paragraph.ParentList[itemIndex] = temp;
            }

            RefreshParagraphsUIAfterSwap();
        }

        public void MoveParagraphDown()
        {
            if (Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph)) == Paragraph.ParentList.Count - 1)
            {
                for (int i = Paragraph.ParentList.Count - 1; i > 0; i--)
                {
                    Paragraph.ParentList[i] = Paragraph.ParentList[i - 1];
                }

                Paragraph.ParentList[0] = Paragraph;
            }
            else
            {
                int itemIndex = Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph));

                Paragraph temp = Paragraph.ParentList[itemIndex + 1];
                Paragraph.ParentList[itemIndex + 1] = Paragraph;
                Paragraph.ParentList[itemIndex] = temp;
            }

            RefreshParagraphsUIAfterSwap();
        }
    }
}
