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
using System.Windows.Media.Animation;

using ApplicationLib.Interfaces;

namespace ApplicationLib.Views
{
    public partial class ParagraphElementSettings : UserControl, IParagraphSettings
    {
        #region IParagraphSettings
        public Action OnParagraphDelete { get; set; }
        public Action OnParagraphShowOrHideHint { get; set; }
        public Action MoveParagraphUp { get; set; }
        public Action MoveParagraphDown { get; set; }
        #endregion

        #region Properties
        private Image StaticDeleteIcon { get; }
        private Image ActiveDeleteIcon { get; }
        private TextBlock ConfirmDeletionTextBlock { get; }
        private Grid DeleteParagraphIconsGrid { get; }
        #endregion

        public ParagraphElementSettings()
        {
            InitializeComponent();

            StaticDeleteIcon = deleteParagraphImageStatic;
            ActiveDeleteIcon = deleteParagraphImageActive;
            ConfirmDeletionTextBlock = deletionConfirmationTextBox;
            DeleteParagraphIconsGrid = deleteParagraphIconsGrid;
        }

        #region Event handlers
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

        private void ShowOrHideParagraphHint(object sender, MouseButtonEventArgs e)
        {
            OnParagraphShowOrHideHint();
        }

        private void ReplaceParagraph(object sender, MouseButtonEventArgs e)
        {
            reorderIconsPanel.Visibility = Visibility.Visible;
            reorderEntryGrid.Visibility = Visibility.Collapsed;
        }

        private void GoToDeletionConfirmation(object sender, MouseButtonEventArgs e)
        {
            HideDeleteIcon();
        }

        private void HideDeleteIcon()
        {
            ActiveDeleteIcon.Visibility = Visibility.Visible;

            ActiveDeleteIcon.BeginAnimation(OpacityProperty,
                GetFadeOutAnimation((sender, e) =>
                {
                    DeleteParagraphIconsGrid.Visibility = Visibility.Collapsed;
                    ConfirmDeletionTextBlock.Visibility = Visibility.Visible;
                }));
        }

        private DoubleAnimation GetFadeOutAnimation(EventHandler onConpletedAnimation)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.5,
                SpeedRatio = 20,
                From = 1,
                FillBehavior = FillBehavior.Stop
            };
            fadeOutAnimation.Completed += onConpletedAnimation;

            return fadeOutAnimation;
        }

        private void DeletionConfirmationTextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            ConfirmDeletionTextBlock.Visibility = Visibility.Collapsed;

            ActiveDeleteIcon.Visibility = Visibility.Collapsed;
            StaticDeleteIcon.Visibility = Visibility.Visible;
            DeleteParagraphIconsGrid.Visibility = Visibility.Visible;
        }

        private void DeleteParagraph(object sender, MouseButtonEventArgs e)
        {
            OnParagraphDelete();
        }

#warning add this to the list
        private void ReorderIconsGridMouseLeave(object sender, MouseEventArgs e)
        {
            reorderIconsPanel.Visibility = Visibility.Collapsed;
            reorderEntryGrid.Visibility = Visibility.Visible;
        }

#warning add this to the list
        private void MoveParagraphDownClick(object sender, MouseButtonEventArgs e)
        {
            MoveParagraphDown();
        }
#warning add this to the list
        private void MoveParagraphUpClick(object sender, MouseButtonEventArgs e)
        {
            MoveParagraphUp();
        }
    }
}
