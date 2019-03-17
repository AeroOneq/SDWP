using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApplicationLib.Models;
using ApplicationLib.Views;

namespace SDWP
{
    public partial class MainPage : Page
    {
        private Documentation Documentation { get; } = new Documentation()
        {
            Name = "SDWP Documentation",
            CreationDate = DateTime.Now,
            AuthorName = "Степанов Евгений"
        };
        private List<Document> Documents { get; } = new List<Document>()
        {
            new Document(){Name = "Техническое задание"},
            new Document(){Name = "Программа испытаний"},
        };

        public MainPage()
        {
            InitializeComponent();
            UploadDocumentationDataToUI();
        }

        #region Upload new documentation
        private void UploadDocumentationDataToUI()
        {
            for (int i = 0; i<Documents.Count; i++)
            {
                DocumentMenuOption documentMenuOption = new DocumentMenuOption(Documents[i]);

                documentMenuOption.Margin = new Thickness(0, 20 + 40 * i, 0, 0);
                documentMenuOption.DocumentBtn.Click += DocumentOptionClick;

                documentsListGrid.Children.Add(documentMenuOption);
            }
        }
        #endregion

        #region Document option click events
        private void DocumentOptionClick(object sender, EventArgs e)
        {

        }
        #endregion

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

        private void HideLeftDocumentsGrid(object sender, MouseButtonEventArgs e)
        {
            MainPageAnimations.HideLeftDocumentationGrid(leftDocumentsGrid);
            MainPageAnimations.AnimateMargin(paragraphsGrid, new Thickness(60, 0, 0, 0));
            MainPageAnimations.AnimateMargin(contentGrid, new Thickness(310, 0, 0, 0));

            hideImagesGrid.Visibility = Visibility.Collapsed;
            showImagesGrid.Visibility = Visibility.Visible;
        }
        private void ShowLeftDocumentsGrid(object sender, MouseButtonEventArgs e)
        {
            MainPageAnimations.AnimateMargin(paragraphsGrid, new Thickness(250, 0, 0, 0));
            MainPageAnimations.AnimateMargin(contentGrid, new Thickness(500, 0, 0, 0));
            MainPageAnimations.ShowLeftDocumentationGrid(leftDocumentsGrid);

            hideImagesGrid.Visibility = Visibility.Visible;
            showImagesGrid.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
