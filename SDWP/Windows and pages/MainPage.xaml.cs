using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;

using FileLib.Interfaces;
using FileLib.FileParsers;

using SDWP.Factories;

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
        public List<Document> Documents { get; } = new List<Document>()
        {
            new Document()
            {
                Name = "Техническое задание",
                Items = new List<Item>()
                {
                    new Item()
                    {
                        Name="Супер введение",
                        Items = new List<Item>()
                        {
                            new Item()
                            {
                                Name="Введение",
                                Items = null,
                                Paragraphs = new List<IParagraphElement>()
                                {
                                    new Subparagraph(new Item())
                                    {
                                        Text = " asdsadasdsadsadasd", 
                                        Hint = "HINT"
                                    },
                                    new Subparagraph(new Item())
                                    {
                                        Text = " asdsadasdsadsadasd",
                                        Hint = "HINT"
                                    },
                                }
                            }
                        },
                        Paragraphs = null,
                    },
                    new Item()
                    {
                        Name="Основная часть",
                        Items = null,
                        Paragraphs = new List<IParagraphElement>()
                        {
                            new Subparagraph( new Item())
                            {
                                Text = "asdasdasdasdasdasdasdasdasdasdasd"
                            }
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

        private StackPanel DocumentsListStackPanel { get; set; }
        private StackPanel ParagraphsListPanel { get; set; }
        private StackPanel ItemsListStackPanel { get; set; }
        private Grid ParagraphElementsGrid { get; set; }
        private Grid AddNewParagraphElementGrid { get; set; }

        private string ParagraphSearchTextBoxDefaultTetx { get; } = "Введите запрос...";

        private ISdwpAbstractFactory AbstractFactory { get; }
        private IDocController DocController { get; set; }
        #endregion

        public MainPage()
        {
            InitializeComponent();
            AbstractFactory = new SdwpAbstractFactory();

#warning TEST
            Documents[0].Items[0].Items[0].ParentItem = Documents[0].Items[0];
            Documents[0].Items[0].Items[0].ParentList = Documents[0].Items[0].Items;
            Documents[0].Items[0].ParentList = Documents[0].Items;
            Documents[0].Items[0].ParentItem = null;

            SetPropertiesValue();
            UploadDocumentation(Documentation, Documents);
        }

        private void SetPropertiesValue()
        {
            LeftDocumentationGrid = leftDocumentsGrid;
            ItemsGrid = itemsGrid;
            ParagraphsGrid = paragraphsGrid;

            HideLeftGridImagesGrid = hideLeftGridImagesGrid;
            ShowLeftGridImagesGrid = showLeftGridImagesGrid;

            DocumentsListStackPanel = documentsListStackPanel;
            ParagraphsListPanel = paragraphsListPanel;
            ItemsListStackPanel = itemsListStackPanel;
            ParagraphElementsGrid = paragraphElementsGrid;
            AddNewParagraphElementGrid = addNewParagraphElementGrid;
        }

        #region Upload new documentation
        private void UploadDocumentation(Documentation documentation, List<Document> documents)
        {
            DocController = AbstractFactory.GetDocController(documentation, documents);

            UploadDocumentationDataToUI(DocController.Documentation);
            UploadDocumentsDataToUI(DocController.Documents);
        }

        private void UploadDocumentationDataToUI(Documentation documentation)
        {
#warning Create Upload logic
        }

        /// <summary>
        /// Uploads all documents which are in the current documentation to the left
        /// document grid, and creates a PreviewMouseDown Event for every document
        /// visual object
        /// </summary>
        private void UploadDocumentsDataToUI(List<Document> documents)
        {
            DocumentsListStackPanel.Children.Clear();

            for (int i = 0; i < documents.Count; i++)
            {
                DocumentMenuOption documentMenuOption = new DocumentMenuOption(documents[i],
                    documents);

                //set events
                documentMenuOption.OnDocumentItemClick += UploadDocumentItems;
                documentMenuOption.UpdateList += RefreshDocumentUI;

                DocumentsListStackPanel.Children.Add(documentMenuOption);
            }
        }
        #endregion

        #region Refresh UI methods
        /// <summary>
        /// Updates the current item's list ui (uploads the items of a CurrentList again), 
        /// if the current item's list is not null. Usualy this methods are used in events
        /// </summary>
        private void RefreshItemsUI()
        {
            if (DocController.CurrentItemsList != null)
                UploadItemsToPanel(DocController.CurrentItemsList);
        }

        private void RefreshParagraphsUI()
        {
            if (DocController.CurrentParagraphsList != null)
                UploadParagraphsToParagraphsListPanel(DocController.CurrentParagraphsList);
        }

        private void RefreshDocumentUI()
        {
            if (DocController.Documents != null)
                UploadDocumentsDataToUI(DocController.Documents);
        }
        #endregion

        #region Option click events
        /// <summary>
        /// Uploads all visualizations of Items to the ItemsListStackPanel, creates a PreviewMouseDown event
        /// (to upload the content of item when clicked) and then 
        /// begins the animation to show this grid
        /// </summary>
        private void UploadDocumentItems(object sender, EventArgs e)
        {
            DocumentMenuOption clickedDocumentMenuOption = sender as DocumentMenuOption;
            ChangeBackgroundsOfDocumentBtns(clickedDocumentMenuOption);

            Document document = clickedDocumentMenuOption.Document;

            DocController.UploadDocument(document);
            UploadItemsToPanel(document.Items);
        }

        /// <summary>
        /// Uploads all items to the itmes list panel,
        /// for each item the parent list is defined
        /// </summary>
        /// <param name="items"></param>
        private void UploadItemsToPanel(List<Item> items)
        {
            ItemsListStackPanel.Visibility = Visibility.Collapsed;
            ItemsListStackPanel.Width = 0;
            ItemsListStackPanel.Children.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                UploadSingleItemToPanel(items[i]);
                items[i].ParentList = items;
            }

            ItemsListStackPanel.Visibility = Visibility.Visible;
            MainPageAnimations.AnimateWidth(ItemsListStackPanel, 250);
        }

        private void UploadSingleItemToPanel(Item item)
        {
            ItemMenuOption itemMenuOption = new ItemMenuOption(item);

            //set events
            itemMenuOption.OnItemClick += UploadItemData;
            itemMenuOption.UpdateList += RefreshItemsUI;

            ItemsListStackPanel.Children.Add(itemMenuOption);
        }

        /// <summary>
        /// After the click on an document element changes the background of this document item to 
        /// the color of the ItemsGrid
        /// </summary>
        private void ChangeBackgroundsOfDocumentBtns(DocumentMenuOption clickedOption)
        {
            foreach (FrameworkElement el in DocumentsListStackPanel.Children)
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
        /// If we upload items then we must set a backToPreviousItemTextBlock text (so we can get to the
        /// previous item list)
        /// </summary>
        private void UploadItemData(object sender, EventArgs e)
        {
            ItemMenuOption clickedItemMenuOption = sender as ItemMenuOption;
            Item item = clickedItemMenuOption.Item;

            ChangeBackgroundOfItemBtns(clickedItemMenuOption);
            DocController.UploadItem(item);

            if (item.Items != null)
            {
                UploadItemsToPanel(DocController.CurrentItem.Items);
                backToPreviousItemTextBlock.Text = "к " + item.Name;

                if (DocController.CurrentItem.ParentList != null)
                    backToPreviousItemStaticImage.IsEnabled = true;
            }
            else
            {
                UploadParagraphsToParagraphsListPanel(item.Paragraphs);
            }
        }

        /// <summary>
        /// Uploads all paragraphs of a current item to the paragraphs stack panel
        /// </summary>
        private void UploadParagraphsToParagraphsListPanel(List<IParagraphElement> paragraphs)
        {
            ParagraphsListPanel.Children.Clear();

            for (int i = 0; i < paragraphs.Count; i++)
            {
                UserControl paragraphView = paragraphs[i].GetEditView();
                paragraphView.Margin = new Thickness(0, 10, 0, 0);

                (paragraphView as IParagraphEditView).ParentList = paragraphs;
                (paragraphView as IParagraphEditView).RefreshParagraphsUI = RefreshParagraphsUI;

                ParagraphsListPanel.Children.Add(paragraphView);
            }
        }

        /// <summary>
        /// After the click on an items element changes the background of this item to 
        /// the color of the ParagraphsGrid
        /// </summary>
        private void ChangeBackgroundOfItemBtns(ItemMenuOption clickedItem)
        {
            foreach (FrameworkElement el in ItemsListStackPanel.Children)
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

        /// <summary>
        /// If the current item has a parent and parent has item's list, 
        /// then we must upload get back to the parent item
        /// </summary>
        private void GoToPreviousItem(object sender, MouseButtonEventArgs e)
        {
            if (DocController.CanGoToPrevItem())
            {
                UploadItemsToPanel(DocController.CurrentItem.ParentList);

                //update text of a backToPreviousItemTextBlock and if there is no parent disable the back img
                if (DocController.CurrentItem.ParentItem != null)
                {
                    backToPreviousItemTextBlock.Text = "к " + DocController.CurrentItem.ParentItem.Name;
                }
                else
                {
                    backToPreviousItemTextBlock.Text = string.Empty;
                    backToPreviousItemStaticImage.IsEnabled = false;
                }

                DocController.GoToPreviousItem(DocController.CurrentItem);
            }
        }

        /// <summary>
        /// Starts the process of creation a new item in the current item
        /// </summary>
        private void CreateNewItem(object sender, MouseButtonEventArgs e)
        {
            if (DocController.CurrentItemsList != null)
            {
                CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(
                    DocController.CurrentItemsList, DocController.CurrentItem);
                createNewItemWindow.ShowDialog();

                UploadItemsToPanel(DocController.CurrentItemsList);
            }
            else
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала загрузите документ либо пункт",
                    MessageBoxButton.OK);
            }
        }

        #region Add new paragraphs (IParagraphElement) methods
        private void AddNewSubparagraph(object sender, MouseEventArgs e)
        {
            if (DocController.CurrentContentItem != null)
            {
                Item contentItem = DocController.CurrentContentItem;

                CreateNewSubparagraphWindow createWindow = new CreateNewSubparagraphWindow(contentItem);
                createWindow.ShowDialog();

                if (createWindow.DialogResult == true)
                    RefreshParagraphsUI();
            }
        }

        private void AddNewTable(object sender, MouseEventArgs e)
        {
            if (DocController.CurrentContentItem != null)
            {
                Item contentItem = DocController.CurrentContentItem;

                CreateNewTableWindow createWindow = new CreateNewTableWindow(contentItem);
                createWindow.ShowDialog();

                if (createWindow.DialogResult == true)
                    RefreshParagraphsUI();
            }
        }
        #endregion
    }
}
