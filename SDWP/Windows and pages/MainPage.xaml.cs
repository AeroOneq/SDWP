using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ApplicationLib.Models;
using ApplicationLib.Views;
using ApplicationLib.Interfaces;

namespace SDWP
{
    public partial class MainPage : Page
    {
        #region Test
        private Documentation Documentation { get; } = new Documentation()
        {
            Name = "SDWP Documentation",
            CreationDate = DateTime.Now,
            AuthorName = "Степанов Евгений"
        };
        private List<Document> Documents { get; } = new List<Document>()
        {
            new Document()
            {
                Name = "Техническое задание",
                Items = new List<Item>()
                {
                    new Item()
                    {
                        Name="Введение",
                        Items = null,
                        Paragraphs = new List<IParagraphElement>()
                        {
                            new Subparagraph("qwertyqwertyqwertyy", new Item())
                        }
                    }
                }
            },
        };
        #endregion

        #region Properties
        private Grid LeftDocumentationGrid { get; set; }
        private Grid ItemsGrid { get; set; }
        private Grid ParagraphsGrid { get; set; }

        private Grid HideLeftGridImagesGrid { get; set; } 
        private Grid ShowLeftGridImagesGrid { get; set; }

        private Grid ParagraphElementsGrid { get; set; } 
        private Grid AddNewParagraphElementGrid { get; set; }
        #endregion

        public MainPage()
        {
            InitializeComponent();

            SetPropertiesValue();
            UploadDocumentationDataToUI();
        }

        private void SetPropertiesValue()
        {
            LeftDocumentationGrid = leftDocumentsGrid;
            ItemsGrid = itemsGrid;
            ParagraphsGrid = paragraphsGrid;

            HideLeftGridImagesGrid = hideLeftGridImagesGrid;
            ShowLeftGridImagesGrid = showLeftGridImagesGrid;

            ParagraphElementsGrid = paragraphElementsGrid;
            AddNewParagraphElementGrid = addNewParagraphElementGrid;
        }

        #region Upload new documentation
        private void UploadDocumentationDataToUI()
        {
            for (int i = 0; i < Documents.Count; i++)
            {
                DocumentMenuOption documentMenuOption = new DocumentMenuOption(Documents[i])
                {
                    Margin = new Thickness(0, 20 + 40 * i, 0, 0)
                };
                documentMenuOption.PreviewMouseDown += UploadDocumentItems;

                documentsListGrid.Children.Add(documentMenuOption);
            }
        }
        #endregion

        #region Option click events
        private void UploadDocumentItems(object sender, EventArgs e)
        {
            DocumentMenuOption clickedDocumentMenuOption = sender as DocumentMenuOption;
            ChangeBackgroundsOfDocumentBtns(clickedDocumentMenuOption);

            Document document = clickedDocumentMenuOption.Document;

            for (int i = 0; i < document.Items.Count; i++)
            {
                ItemMenuOption itemMenuOption = new ItemMenuOption(document.Items[i])
                {
                    Margin = new Thickness(0, 20 + 40 * i, 0, 0)
                };
                itemMenuOption.PreviewMouseDown += UploadItemData;

                itemsListGrid.Children.Add(itemMenuOption);
            }
        }

        private void ChangeBackgroundsOfDocumentBtns(DocumentMenuOption clickedOption)
        {
            foreach (FrameworkElement el in documentsListGrid.Children)
            {
                if (el is DocumentMenuOption)
                    (el as DocumentMenuOption).DocumentBtn.Background = new SolidColorBrush(Colors.Transparent);
            }

            Color clickedBtnBackgroundColor = (Color)Application.Current.Resources["mainPageItemsGridBackgroundColor"];
            clickedOption.DocumentBtn.Background = new SolidColorBrush(clickedBtnBackgroundColor);
        }

        private void UploadItemData(object sender, EventArgs e)
        {
            ItemMenuOption clickedItemMenuOption = sender as ItemMenuOption;
            Item item = clickedItemMenuOption.Item;

            ChangeBackgroundOfItemBtns(clickedItemMenuOption);

            if (item.Items != null)
            {
#warning Create logic for appending other items here
            }
            else
            {
                for (int i = 0; i < item.Paragraphs.Count; i++)
                {
                    FrameworkElement paragraphView = item.Paragraphs[i].GetEditView();
                    paragraphView.Margin = new Thickness(15, 0, 15, 0);

                    paragraphsListGrid.Children.Add(paragraphView);
                }
            }
        }

        private void ChangeBackgroundOfItemBtns(ItemMenuOption clickedItem)
        {
            foreach (FrameworkElement el in itemsListGrid.Children)
            {
                if (el is ItemMenuOption)
                    (el as ItemMenuOption).ItemBtn.Background = new SolidColorBrush(Colors.Transparent);
            }

            Color clickedBtnBackgroundColor = (Color)Application.Current.Resources["mainPageParagraphsGridBackgroundColor"];
            clickedItem.ItemBtn.Background = new SolidColorBrush(clickedBtnBackgroundColor);
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
            MainPageAnimations.HideLeftDocumentationGrid(LeftDocumentationGrid);
            MainPageAnimations.AnimateMargin(ItemsGrid, new Thickness(60, 0, 0, 0));
            MainPageAnimations.AnimateMargin(ParagraphsGrid, new Thickness(310, 0, 0, 0));

            HideLeftGridImagesGrid.Visibility = Visibility.Collapsed;
            ShowLeftGridImagesGrid.Visibility = Visibility.Visible;
        }

        private void ShowLeftDocumentsGrid(object sender, MouseButtonEventArgs e)
        {
            MainPageAnimations.AnimateMargin(ItemsGrid, new Thickness(250, 0, 0, 0));
            MainPageAnimations.AnimateMargin(ParagraphsGrid, new Thickness(500, 0, 0, 0));
            MainPageAnimations.ShowLeftDocumentationGrid(LeftDocumentationGrid);

            HideLeftGridImagesGrid.Visibility = Visibility.Visible;
            ShowLeftGridImagesGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowParagraphElementsAddOptions(object sender, MouseButtonEventArgs e)
        {
            Size paragraphElementsGridNewSize = new Size(200, 35);

            AddNewParagraphElementGrid.Visibility = Visibility.Collapsed;
            ParagraphElementsGrid.Visibility = Visibility.Visible;

            MainPageAnimations.AnimateSize(ParagraphElementsGrid, paragraphElementsGridNewSize,
                new Action(()=> { }));
        }
        #endregion

        private void ParagraphElementsGridMouseLeave(object sender, MouseEventArgs e)
        {
            Size paragraphElementsGridNewSize = new Size(0, 35);

            MainPageAnimations.AnimateSize(ParagraphElementsGrid, paragraphElementsGridNewSize,
                new Action(()=> 
                {
                    AddNewParagraphElementGrid.Visibility = Visibility.Visible;
                    ParagraphElementsGrid.Visibility = Visibility.Collapsed;
                }));
        }
    }
}
