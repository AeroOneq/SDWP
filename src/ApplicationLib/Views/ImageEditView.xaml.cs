﻿using System;
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
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

using Microsoft.Win32;

namespace ApplicationLib.Views
{
    public partial class ImageEditView : UserControl, IParagraphEditView
    {
        #region Properties
        private ParagraphImage ParagraphImage { get; }
        public Paragraph Paragraph { get; }
        private Image MainImage { get; set; }
        private ImageParagraphSettings ImageParagraphSettings { get; }
        private HintControl HintControl { get; set; }
        #endregion

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        public Action RefreshParagraphsUIAfterSwap { get; set; }
        #endregion

        public ImageEditView(Paragraph paragraph)
        {
            InitializeComponent();

            Paragraph = paragraph;
            ParagraphImage = Paragraph.ParagraphElement as ParagraphImage;
            MainImage = mainImage;
            HintControl = hintControl;
            ImageParagraphSettings = imageParagraphsSettings;

            SetParagraphSettingsEvents();
            SetImageSource(ParagraphImage.ImageSource);

            DataContext = ParagraphImage;
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

            imgSettings.MoveParagraphUp += MoveParagraphUp;
            imgSettings.MoveParagraphDown += MoveParagraphDown;
            imgSettings.OnParagraphDelete += DeleteParagraph;
            imgSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
            imgSettings.OnUploadNewImage += SelectAndUploadNewImage;
        }

        #region IParagraphEditView methods
        public void DeleteParagraph()
        {
            (Paragraph as IParentableParagraph).RemoveParagraphFromParentList();
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

        public void SelectAndUploadNewImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Выберете рисунок для зкагрузки",
                CheckFileExists = true,
                Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*png",
                ValidateNames = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
                UploadImage(openFileDialog.FileName);
        }

        private void UploadImage(string imagePath)
        {
            byte[] imageByteArr;
            using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);
                imageByteArr = new byte[fs.Length];
                fs.Read(imageByteArr, 0, (int)fs.Length);

                ParagraphImage.ImageSource = imageByteArr;
                SetImageSource(imageByteArr);
            }
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
        #endregion
    }
}
