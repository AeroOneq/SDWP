using ApplicationLib.Interfaces;
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

namespace ApplicationLib.Views
{
    /// <summary>
    /// Логика взаимодействия для ImageParagraphSettings.xaml
    /// </summary>
    public partial class ImageParagraphSettings : UserControl, IImageSettings
    {
        #region IParagraphSettings
        public Action OnParagraphDelete { get; set; }
        public Action OnParagraphShowOrHideHint { get; set; }
        public Action OnParagraphReplace { get; set; }
        public Action OnUploadNewImage { get; set; }
        #endregion

        public ImageParagraphSettings()
        {
            InitializeComponent();
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
            OnParagraphReplace();
        }

        private void DeletePragraph(object sender, MouseButtonEventArgs e)
        {
            OnParagraphDelete();
        }

        private void UploadNewImage(object sender, MouseButtonEventArgs e)
        {
            OnUploadNewImage();
        }
    }
}
