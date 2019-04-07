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
using System.IO;

namespace ApplicationLib.Views
{
    public partial class ImageEditView : UserControl, IParagraphEditView
    {
        #region Properties
        private ParagraphImage ParagraphImage { get; }
        private Image MainImage { get; set; }
        private ImageParagraphSettings ImageParagraphSettings { get;  }
        private HintControl HintControl { get; set; }
        #endregion

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        public List<IParagraphElement> ParentList { get; set; }
        #endregion

        public ImageEditView(ParagraphImage paragraphImage)
        {
            InitializeComponent();

            ParagraphImage = paragraphImage;
            MainImage = mainImage;
            HintControl = hintControl;
            ImageParagraphSettings = imageParagraphsSettings;

            SetParagraphSettingsEvents();
            SetImageSource(ParagraphImage.ImageSource);
        }

        private void SetImageSource(byte[] imageSource)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream(imageSource))
            {
                ms.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            bitmapImage.Freeze();

            MainImage.Source = bitmapImage;
        }

        private void SetParagraphSettingsEvents()
        {
            IImageSettings imgSettings = ImageParagraphSettings as IImageSettings;

            imgSettings.OnParagraphDelete += DeleteParagraph;
            imgSettings.OnParagraphReplace += ReplaceParagraph;
            imgSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
        }

        #region IParagraphEditView methods
        public void DeleteParagraph()
        {
            ParentList.Remove(ParagraphImage);
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
    }
}
