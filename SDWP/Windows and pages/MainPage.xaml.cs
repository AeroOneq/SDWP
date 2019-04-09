using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;

using SDWP.Factories;
using SDWP.Models;

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
                                    new Subparagraph(" asdsadasdsadsadasd")
                                    {
                                        Hint = "HINT",
                                        Title = "P1"
                                    },
                                    new Subparagraph(" asdsadasdsadsadasd")
                                    {
                                        Hint = "HINT",
                                        Title = "P2"
                                    },
                                    new NumberedList(new List<NumberedListElement>()
                                    {
                                        new NumberedListElement("asdsadasdasd"),
                                        new NumberedListElement("asdsadasdasd"),
                                        new NumberedListElement("asdsadasdasd"),
                                        new NumberedListElement("asdsadasdasd"),
                                    })
                                    {
                                        Title = "Numbered LIST"
                                    }
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
                           new Subparagraph(" asdsadasdsadsadasd")
                           {
                               Hint = "HINT",
                               Title = "P3"
                           },
                           new ParagraphImage(new byte[0])
                           {
                               Title = "P4"
                            },
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

        private Image BackToPreviousItemStaticImage { get; set; }

        private StackPanel DocumentsListStackPanel { get; set; }
        private StackPanel ParagraphsListPanel { get; set; }
        private StackPanel ItemsListStackPanel { get; set; }
        private Grid ParagraphElementsGrid { get; set; }
        private Grid AddNewParagraphElementGrid { get; set; }

        private TextBox GoToTreeViewModeTextBox { get; set; }
        private TextBox GoToListModeTextBox { get; set; }
        private TreeView DocumentTreeView { get; set; }
        private ScrollViewer ListModeScroll { get; set; }

        private string ParagraphSearchTextBoxDefaultTetx { get; } = "Введите запрос...";

        private ISdwpAbstractFactory AbstractFactory { get; }
        private IDocController DocController { get; set; }
        #endregion

        public MainPage()
        {
            InitializeComponent();
            AbstractFactory = new SdwpAbstractFactory();

            byte[] imgByteArr;
            using (var ms = new FileStream(@"C:\Users\Aero\Desktop\python\s1200.jpg", FileMode.Open, FileAccess.Read))
            {
                ms.Seek(0, SeekOrigin.Begin);
                imgByteArr = new byte[ms.Length];
                ms.Read(imgByteArr, 0, (int)ms.Length);
            }

            (Documents[0].Items[1].Paragraphs[1] as ParagraphImage).ImageSource = imgByteArr;

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

            BackToPreviousItemStaticImage = backToPreviousItemStaticImage;

            DocumentsListStackPanel = documentsListStackPanel;
            ParagraphsListPanel = paragraphsListPanel;
            ItemsListStackPanel = itemsListStackPanel;
            ParagraphElementsGrid = paragraphElementsGrid;
            AddNewParagraphElementGrid = addNewParagraphElementGrid;

            GoToTreeViewModeTextBox = goToTreeViewModeTextBox;
            GoToListModeTextBox = goToListModeTextBox;
            DocumentTreeView = documentTreeView;
            ListModeScroll = listModelScroll;
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

        #region Document Tree View
        private void TreeViewItemMouseDown(object sender, RoutedEventArgs e)
        {
            (sender as DocTreeViewItem).IsSelected = true;
        }

        private void OnTreeViewRenameItem(object sender, RoutedEventArgs e)
        {
            DocTreeViewItem docTreeViewItem = DocumentTreeView.SelectedItem as DocTreeViewItem;

            docTreeViewItem.IsEnabledForEdditing = true;
        }

        private void OnTreeContextMenuAddNewItem(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewItemLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as DocTreeViewItem).IsEnabledForEdditing = false;
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
                UploadItemsToPanel(DocController.CurrentItem, DocController.CurrentItemsList);
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
            UploadItemsToPanel(null, document.Items);
            CreateDocumentTreeView(document);
        }

        /// <summary>
        /// Creates the document tree view (the second way to observe the document)
        /// </summary>
        private void CreateDocumentTreeView(Document document)
        {
            DocumentTreeView.Items.Clear();
            foreach (Item item in document.Items)
            {
                DocTreeViewItem treeViewItem = CreateNewListItem(item);

                UploadItemsToTreeView(item, treeViewItem);

                DocumentTreeView.Items.Add(treeViewItem);
            }
        }

        private DocTreeViewItem CreateNewListItem(Item item)
        {
            DocTreeViewItem treeItem = new DocTreeViewItem(item);

            if (item.Paragraphs != null)
                treeItem.Style = Resources["treeViewContentItemStyle"] as Style;
            else
                treeItem.Style = Resources["treeViewItemsItemStyle"] as Style;

            treeItem.ContextMenu = Resources["treeViewItemContextMenu"] as ContextMenu;
            treeItem.Margin = new Thickness(10, 0, 0, 0);
            treeItem.MouseDoubleClick += OnTreeViewItemDoubleClick;

            return treeItem;
        }

        private void OnTreeViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DocTreeViewItem treeItem = sender as DocTreeViewItem;
            if (treeItem.Item.Paragraphs != null)
            {
                DocController.CurrentContentItem = treeItem.Item;
                DocController.CurrentParagraphsList = treeItem.Item.Paragraphs;

                UploadParagraphsToParagraphsListPanel(treeItem.Item.Paragraphs);
            }
        }
            
        /// <summary>
        /// The recursive algorithm which creates the tree view for a selected doc.
        /// Firstly we upload all "item" items and then upload the content of the item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rootItem"></param>
        private void UploadItemsToTreeView(Item item, DocTreeViewItem rootItem)
        {
            if (item.Items != null)
                foreach (Item i in item.Items)
                {
                    DocTreeViewItem treeViewItem = CreateNewListItem(i);

                    rootItem.Items.Add(treeViewItem);

                    if (i.Items != null)
                    {
                        foreach (Item ii in i.Items)
                        {
                            UploadItemsToTreeView(ii, treeViewItem);
                        }
                    }
                }
        }

        /// <summary>
        /// Uploads all items to the itmes list panel,
        /// for each item the parent list is defined
        /// </summary>
        /// <param name="items"></param>
        private void UploadItemsToPanel(Item parentItem, List<Item> items)
        {
            ItemsListStackPanel.Visibility = Visibility.Collapsed;
            ItemsListStackPanel.Width = 0;
            ItemsListStackPanel.Children.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                UploadSingleItemToPanel(items[i]);
                (items[i] as IParentableItem).SetParents(parentItem, items);
            }

            ItemsListStackPanel.Visibility = Visibility.Visible;
            MainPageAnimations.AnimateWidth(ItemsListStackPanel, 250);
        }

        private void UploadSingleItemToPanel(Item item)
        {
            ItemMenuOption itemMenuOption = new ItemMenuOption(item);
            itemMenuOption.UpdateList += RefreshItemsUI;

            //set events
            itemMenuOption.OnItemClick += UploadItemData;

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

            item.SetParents(DocController.CurrentItem,
                DocController.CurrentItemsList);

            ChangeBackgroundOfItemBtns(clickedItemMenuOption);
            DocController.UploadItem(item);

            if (item.Items != null)
            {
                UploadItemsToPanel(DocController.CurrentItem, DocController.CurrentItem.Items);
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
                (paragraphs[i] as IParentableParagraph).SetParents(DocController.CurrentContentItem,
                    DocController.CurrentContentItem.Paragraphs);

                UserControl paragraphView = paragraphs[i].GetEditView();
                paragraphView.Margin = new Thickness(0, 10, 0, 0);

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

        private void TreeViewOptionTextBoxMouseEnter(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.TextDecorations.Add(TextDecorations.Underline);
        }

        private void TreeViewOptionTextBoxMouseLeave(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.TextDecorations.Clear();
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
                UploadItemsToPanel(DocController.CurrentItem.ParentItem,
                    DocController.CurrentItem.ParentList);

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

                UploadItemsToPanel(DocController.CurrentItem, DocController.CurrentItemsList);
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

        private void AddNewNumberedList(object sender, MouseEventArgs e)
        {
            if (DocController.CurrentContentItem != null)
            {
                Item contentItem = DocController.CurrentContentItem;

                CreateNewNumberedListWindow createWindow = new CreateNewNumberedListWindow(contentItem);
                createWindow.ShowDialog();

                if (createWindow.DialogResult == true)
                    RefreshParagraphsUI();
            }
        }

        private void AddNewParagraphImage(object sender, MouseButtonEventArgs e)
        {
            if (DocController.CurrentContentItem != null)
            {
                Item contentItem = DocController.CurrentContentItem;

                CreateNewImageWindow createNewImageWindow = new CreateNewImageWindow(contentItem);
                createNewImageWindow.ShowDialog();

                if (createNewImageWindow.DialogResult == true)
                    RefreshParagraphsUI();
            }
        }
        #endregion

        private void GoToListMode(object sender, MouseButtonEventArgs e)
        {
            backToPreviousItemStaticImage.IsEnabled = true;
            DocumentTreeView.Visibility = Visibility.Collapsed;
            ListModeScroll.Visibility = Visibility.Visible;

            GoToListModeTextBox.Visibility = Visibility.Collapsed;
            GoToTreeViewModeTextBox.Visibility = Visibility.Visible;
        }

        private void GoToTreeViewMode(object sender, MouseButtonEventArgs e)
        {
            backToPreviousItemStaticImage.IsEnabled = false;
            DocumentTreeView.Visibility = Visibility.Visible;
            ListModeScroll.Visibility = Visibility.Collapsed;

            GoToListModeTextBox.Visibility = Visibility.Visible;
            GoToTreeViewModeTextBox.Visibility = Visibility.Collapsed;
        }
    }
}
