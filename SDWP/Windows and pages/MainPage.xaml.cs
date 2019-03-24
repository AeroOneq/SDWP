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
                            new Subparagraph("qwertyqwertyqwertyy", new Item()),
                            new Subparagraph("qwe ewq qwe eq qwe qwe qwe qwe ", new Item())
                        }
                    },
                    new Item()
                    {
                        Name="Основная часть",
                        Items = null,
                        Paragraphs = new List<IParagraphElement>()
                        {
                            new Subparagraph("asdasdasdasdasdasdasdasdasdasdasd", new Item())
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

        private StackPanel ParagraphsListPannel { get; set; }
        private Grid ItemsListGrid { get; set; }
        private Grid ParagraphElementsGrid { get; set; }
        private Grid AddNewParagraphElementGrid { get; set; }

        private string ParagraphSearchTextBoxDefaultTetx { get; } = "Введите запрос...";
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

            ParagraphsListPannel = paragraphsListPannel;
            ItemsListGrid = itemsListGrid;
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
        /// <summary>
        /// Uploads all visualizations of Items to the ItemsListGrid and then 
        /// begins the animation to show this grid
        /// </summary>
        private void UploadDocumentItems(object sender, EventArgs e)
        {
            ItemsListGrid.Visibility = Visibility.Collapsed;
            ItemsListGrid.Width = 0;

            DocumentMenuOption clickedDocumentMenuOption = sender as DocumentMenuOption;
            ChangeBackgroundsOfDocumentBtns(clickedDocumentMenuOption);

            Document document = clickedDocumentMenuOption.Document;
            ItemsListGrid.Children.Clear();

            for (int i = 0; i < document.Items.Count; i++)
            {
                ItemMenuOption itemMenuOption = new ItemMenuOption(document.Items[i])
                {
                    Margin = new Thickness(0, 20 + 40 * i, 0, 0)
                };
                itemMenuOption.PreviewMouseDown += UploadItemData;

                ItemsListGrid.Children.Add(itemMenuOption);
            }

            ItemsListGrid.Visibility = Visibility.Visible;
            MainPageAnimations.AnimateWidth(ItemsListGrid, 250);
        }

        /// <summary>
        /// After the click on an document element changes the background of this document item to 
        /// the color of the ItemsGrid
        /// </summary>
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

        /// <summary>
        /// After the click on an item element uploads the data of this item to either the paragraphs grid
        /// if this Item object contains paragraphs or to the ItemsGrid, if this item contains Items
        /// </summary>
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
                ParagraphsListPannel.Children.Clear();

                for (int i = 0; i < item.Paragraphs.Count; i++)
                {
                    FrameworkElement paragraphView = item.Paragraphs[i].GetEditView();
                    paragraphView.Margin = new Thickness(0, 10, 0, 0);

                    ParagraphsListPannel.Children.Add(paragraphView);
                }
            }
        }

        /// <summary>
        /// After the click on an items element changes the background of this item to 
        /// the color of the ParagraphsGrid
        /// </summary>
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
            AddNewParagraphElementGrid.Visibility = Visibility.Collapsed;
            ParagraphElementsGrid.Visibility = Visibility.Visible;

            MainPageAnimations.AnimateWidth(ParagraphElementsGrid, 200);
        }
        #endregion

        private void ParagraphElementsGridMouseLeave(object sender, MouseEventArgs e)
        {
            MainPageAnimations.AnimateWidth(ParagraphElementsGrid, 0,
                new Action(() =>
                {
                    AddNewParagraphElementGrid.Visibility = Visibility.Visible;
                    ParagraphElementsGrid.Visibility = Visibility.Collapsed;
                }));
        }

        /// <summary>
        /// If the text of a search text box is a default text (like a placeholder)
        /// we must clear the textbox for a user's purposes
        /// </summary>
        private void ParagraphSearchTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == ParagraphSearchTextBoxDefaultTetx)
                textBox.Text = string.Empty;
        }

        /// <summary>
        /// If the textbox's text is an empty string then we must place a placeholder in that
        /// textbox
        /// </summary>
        private void ParagraphSearchTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == string.Empty)
                textBox.Text = ParagraphSearchTextBoxDefaultTetx;
        }

        /// <summary>
        /// On every text changed we must find all the elements which sutisfiy the search query
        /// </summary>
        private void ParagraphSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
#warning place a search logic here
        }
    }
}
