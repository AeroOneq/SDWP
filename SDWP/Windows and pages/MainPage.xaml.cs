using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Views;

using SDWP.Factories;
using SDWP.Models;
using SDWP.Interfaces;
using System.Threading;

namespace SDWP
{
    public partial class MainPage : Page
    {
        #region Properties
        private Grid LeftDocumentationGrid { get; set; }
        private Grid ItemsGrid { get; set; }
        private Grid ParagraphsGrid { get; set; }

        private Grid HideLeftGridImagesGrid { get; set; }
        private Grid ShowLeftGridImagesGrid { get; set; }

        private Image BackToPreviousItemStaticImage { get; set; }
        private Image AddNewItemStaticImage { get; set; }

        private StackPanel DocumentsListStackPanel { get; set; }
        private StackPanel ParagraphsListPanel { get; set; }
        private StackPanel ItemsListStackPanel { get; set; }
        private Grid ParagraphElementsGrid { get; set; }
        private Grid AddNewParagraphElementGrid { get; set; }

        private TextBox GoToTreeViewModeTextBox { get; set; }
        private TextBox GoToListModeTextBox { get; set; }
        private TreeView DocumentTreeView { get; set; }
        private ScrollViewer ListModeScroll { get; set; }
        private TextBox ParagraphSearchTextBox { get; set; }
#warning add this property to tables
        private StackPanel DocumentationPropertiesStackPannel { get; set; }
#warning add this property to tables
        private CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
#warning add this property to tables    
        private CancellationToken Token { get; set; }
#warning add this property to tables    
        private bool DoWeHaveToCancelParagraphUploading { get; set; }
#warning add this to the tables
        private int NumOfParagraphsPerPanel { get; } = 3;

        private string ParagraphSearchTextBoxDefaultText { get; } = "Введите запрос...";

        private ISdwpAbstractFactory AbstractFactory { get; set; }
        public IDocController DocController { get; set; }

        public LocalDocumentation LocalDocumentation { get; set; }
        #endregion

        public MainPage()
        {
            InitializeComponent();
            InitializeProperties();
            InitializeServices();
        }

        #region Initialize page methods
        private void InitializeServices()
        {
            AbstractFactory = new SdwpAbstractFactory();
            DocController = AbstractFactory.GetDocController();
        }

        private void InitializeProperties()
        {
            LeftDocumentationGrid = leftDocumentsGrid;
            ItemsGrid = itemsGrid;
            ParagraphsGrid = paragraphsGrid;

            HideLeftGridImagesGrid = hideLeftGridImagesGrid;
            ShowLeftGridImagesGrid = showLeftGridImagesGrid;

            BackToPreviousItemStaticImage = backToPreviousItemStaticImage;
            AddNewItemStaticImage = addNewItemStaticImage;

            DocumentsListStackPanel = documentsListStackPanel;
            ParagraphsListPanel = paragraphsListPanel;
            ItemsListStackPanel = itemsListStackPanel;
            ParagraphElementsGrid = paragraphElementsGrid;
            AddNewParagraphElementGrid = addNewParagraphElementGrid;
            ParagraphSearchTextBox = paragraphsSearchTextBox;

            GoToTreeViewModeTextBox = goToTreeViewModeTextBox;
            GoToListModeTextBox = goToListModeTextBox;
            DocumentTreeView = documentTreeView;
            ListModeScroll = listModelScroll;
            DocumentationPropertiesStackPannel = documentationPropertiesStackPannel;
            Token = TokenSource.Token;
        }
        #endregion

        #region Upload new documentation
        public void UploadLocalDocumentation(LocalDocumentation localDocumentation)
        {
            ClearMainPage();

            LocalDocumentation = localDocumentation;

            DocController.UploadLocalDocumentation(LocalDocumentation);
            UploadDocumentation();
        }

        public void UploadCloudDocumentation(Documentation documentation, List<Document> documents)
        {
            ClearMainPage();

            LocalDocumentation = null;

            DocController.UploadCloudDocumentation(documentation, documents);
            UploadDocumentation();
        }

        /// <summary>
        /// Clears all documentations' stack panels and DocController
        /// </summary>
        private void ClearMainPage()
        {
            DocController.Clear();

            DocumentsListStackPanel.Children.Clear();
            ItemsListStackPanel.Children.Clear();
            ParagraphsListPanel.Children.Clear();
        }

        /// <summary>
        /// Uploads documentation to the main page and makes all textboxes with
        /// documentation properties active
        /// </summary>
        private void UploadDocumentation()
        {
            Array.ForEach(DocumentationPropertiesStackPannel.Children.
                OfType<TextBox>().Where(t => t.Name != "documentationCreationDateTextBlock")
                .ToArray(), tx => tx.IsEnabled = true);

            UploadDocumentationDataToUI(DocController.Documentation);
            UploadDocumentsDataToUI(DocController.Documents);
        }

        private void UploadDocumentationDataToUI(Documentation documentation)
        {
            DataContext = documentation;
        }

        /// <summary>
        /// Uploads all documents which are in the current documentation to the left
        /// document grid, and creates a PreviewMouseDown Event for every document
        /// visual object
        /// </summary>
        private void UploadDocumentsDataToUI(List<Document> documents)
        {
            DocumentsListStackPanel.Children.Clear();
            SetDocumentItemParents(documents);

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

        private void SetDocumentItemParents(List<Document> documents)
        {
            foreach (Document document in documents)
            {
                foreach (Item item in document.Items)
                {
                    item.SetParents(null, document.Items);
                    SetItemParents(item);
                }
            }
        }

        private void SetItemParents(Item item)
        {
            if (item.Items != null)
            {
                foreach (Item i in item.Items)
                {
                    i.SetParents(item, item.Items);
                    SetItemParents(i);
                }
            }

            if (item.Paragraphs != null)
            {
                foreach (Paragraph paragraph in item.Paragraphs)
                {
                    (paragraph as IParentableParagraph).SetParents(item, item.Paragraphs);
                    paragraph.ParagraphElement.ParentParagraph = paragraph;
                }
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
            DocTreeViewItem docTreeViewItem = DocumentTreeView.SelectedItem as DocTreeViewItem;
            Item item = docTreeViewItem.Item;

            CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(item.Items, item);
            if (createNewItemWindow.ShowDialog() == true)
                RefreshItemsUI();
        }

        private void OnTreeContextMenuAddNewItemToRoot(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Item> currentItemsList = DocController.CurrentDocument.Items;

                CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(
                    currentItemsList, null);
                if (createNewItemWindow.ShowDialog() == true)
                    RefreshItemsUI();
            }
            catch (NullReferenceException)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала загрузите документ",
                    MessageBoxButton.OK);
            }
        }

        private void TreeViewItemTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).IsEnabled = false;
        }

        private void OnTreeViewDeleteItem(object sender, RoutedEventArgs e)
        {
            DocTreeViewItem docTreeViewItem = DocumentTreeView.SelectedItem as DocTreeViewItem;
            Item selectedItem = docTreeViewItem.Item;

            selectedItem.ParentList.Remove(selectedItem);
            RefreshItemsUI();
        }

        /// <summary>
        /// Creates the document tree view (the second way to observe the document)
        /// </summary>
        private void CreateDocumentTreeView(Document document)
        {
            if (document != null)
            {
                DocumentTreeView.Items.Clear();
                foreach (Item item in document.Items)
                {
                    DocTreeViewItem treeViewItem = CreateNewListItem(item);

                    UploadItemsToTreeView(item, treeViewItem);

                    DocumentTreeView.Items.Add(treeViewItem);
                }
            }
        }

        private DocTreeViewItem CreateNewListItem(Item item)
        {
            DocTreeViewItem treeItem = new DocTreeViewItem(item);

            if (item.Paragraphs != null)
                treeItem.Style = Resources["treeViewContentItemStyle"] as Style;
            else
                treeItem.Style = Resources["treeViewItemsItemStyle"] as Style;

            treeItem.Margin = new Thickness(10, 0, 0, 0);
            treeItem.MouseDoubleClick += OnTreeViewItemDoubleClick;

            return treeItem;
        }

        private void OnTreeViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DocTreeViewItem treeItem = sender as DocTreeViewItem;
            if (treeItem.Item.Paragraphs != null)
            {
                DocController.UploadParagraphs(treeItem.Item);

                CancelParagraphsUploading();
                UploadParagraphsToParagraphsListPanel(treeItem.Item.Paragraphs);
            }
        }

        private void CancelParagraphsUploading()
        {
            if (DoWeHaveToCancelParagraphUploading)
            {
                TokenSource.Cancel();
                ParagraphsListPanel.Children.Clear();
            }
        }

#warning add this to the tables
        private void MoveItemUp(object sender, RoutedEventArgs e)
        {
            if (DocumentTreeView.SelectedItem is DocTreeViewItem treeViewItem)
            {
                Item item = treeViewItem.Item;

                if (item.ParentList.FindIndex(i => i.Equals(item)) == 0)
                {
                    item.ParentList.Remove(item);
                    item.ParentList.Add(item);
                }
                else
                {
                    int itemIndex = item.ParentList.FindIndex(i => i.Equals(item));

                    Item temp = item.ParentList[itemIndex - 1];
                    item.ParentList[itemIndex - 1] = item;
                    item.ParentList[itemIndex] = temp;
                }

                CreateDocumentTreeView(DocController.CurrentDocument);
            }
        }

#warning add this to the tables
        private void MoveItemDown(object sender, RoutedEventArgs e)
        {
            if (DocumentTreeView.SelectedItem is DocTreeViewItem treeViewItem)
            {
                Item item = treeViewItem.Item;

                if (item.ParentList.FindIndex(i => i.Equals(item)) == item.ParentList.Count - 1)
                {
                    for (int i = item.ParentList.Count - 1; i > 0; i--)
                    {
                        item.ParentList[i] = item.ParentList[i - 1];
                    }

                    item.ParentList[0] = item;
                }
                else
                {
                    int itemIndex = item.ParentList.FindIndex(i => i.Equals(item));

                    Item temp = item.ParentList[itemIndex + 1];
                    item.ParentList[itemIndex + 1] = item;
                    item.ParentList[itemIndex] = temp;
                }

                CreateDocumentTreeView(DocController.CurrentDocument);
            }
        }
        #endregion

        #region Refresh UI methods
#warning add this to the table
        private void RefreshParagraphsUIAfterSwap()
        {
            List<Paragraph> paragraphs = DocController.CurrentParagraphsList;
            List<IParagraphEditView> paragraphEditViews = new List<IParagraphEditView>();

            foreach (FrameworkElement fe in ParagraphsListPanel.Children)
            {
                StackPanel stackPanel = fe as StackPanel;

                foreach (FrameworkElement stackPanelChild in stackPanel.Children)
                {
                    paragraphEditViews.Add(stackPanelChild as IParagraphEditView);
                }
            }
        }

        /// <summary>
        /// Updates the current item's list ui (uploads the items of a CurrentList again), 
        /// if the current item's list is not null. Usualy this methods are used in events
        /// </summary>
        private void RefreshItemsUI()
        {
            if (DocumentTreeView.Visibility == Visibility.Collapsed)
            {
                if (DocController.CurrentItemsList != null)
                    UploadItemsToPanel(DocController.CurrentItem, DocController.CurrentItemsList);
            }
            else
            {
                CreateDocumentTreeView(DocController.CurrentDocument);
            }
        }

        private void RefreshParagraphsUI()
        {
            List<Paragraph> paragraphs = DocController.CurrentParagraphsList;

            //try to delete the pragraphEditView if the action was to delete. Only one paragraph can be deleted
            //so when wwe found the deleted paragraph, we can execute the return statement
            if (paragraphs != null)
            {
                CancelParagraphsUploading();

                foreach (FrameworkElement fe in ParagraphsListPanel.Children)
                {
                    StackPanel stackPanel = fe as StackPanel;
                    for (int i = 0; i<stackPanel.Children.Count; i++)
                    {
                        IParagraphEditView paragraphEditView = stackPanel.Children[i] as 
                            IParagraphEditView;

                        if (paragraphs.FindIndex(p => p.Equals(paragraphEditView.Paragraph)) == -1)
                        {
                            stackPanel.Children.Remove(stackPanel.Children[i]);
                            stackPanel.UpdateLayout();
                            return;
                        }
                    }
                }
            }

            //try to add an pragraphEditView if the action was to add. If we reached that part of code
            //that means that the new paragraph was added, but the adding of the paragraph always adds the paragraph
            //to the end of the list, so we must just add a new paragraph view to the end of our main stack panel
            if (ParagraphsListPanel.Children.Count == 0)
                ParagraphsListPanel.Children.Add(new StackPanel());

            StackPanel lastStackPanel = ParagraphsListPanel.Children[ParagraphsListPanel.
                Children.Count - 1] as StackPanel;

            if (lastStackPanel.Children.Count == NumOfParagraphsPerPanel)
            {
                lastStackPanel = new StackPanel();
                ParagraphsListPanel.Children.Add(lastStackPanel);
            }

            lastStackPanel.Children.Add(paragraphs.Last().ParagraphElement.GetEditView());
            lastStackPanel.UpdateLayout();
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
                        UploadItemsToTreeView(i, treeViewItem);
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
                CancelParagraphsUploading();
                UploadParagraphsToParagraphsListPanel(item.Paragraphs);
            }
        }

        /// <summary>
        /// Uploads all paragraphs of a current item to the paragraphs stack panel
        /// </summary>
        private async void UploadParagraphsToParagraphsListPanel(List<Paragraph> paragraphs)
        {
            DoWeHaveToCancelParagraphUploading = true;

            await Dispatcher.BeginInvoke(new Action(() => ParagraphsListPanel.Children.Clear()));

            StackPanel currentPanel = new StackPanel();
            for (int i = 0; i < paragraphs.Count; i++)
            {
                if (i % NumOfParagraphsPerPanel == 0)
                {
                    currentPanel = new StackPanel();
                    ParagraphsListPanel.Children.Add(currentPanel);
                }

                if (Token.IsCancellationRequested)
                {
                    DoWeHaveToCancelParagraphUploading = false;

                    TokenSource = new CancellationTokenSource();
                    Token = TokenSource.Token;

                    return;
                }

                (paragraphs[i] as IParentableParagraph).SetParents(DocController.CurrentContentItem,
                    DocController.CurrentContentItem.Paragraphs);

                UserControl paragraphView = null;
                Dispatcher.Invoke(new Action(() => paragraphView = paragraphs[i].ParagraphElement.GetEditView()));
                await Dispatcher.BeginInvoke(new Action(() =>
                    paragraphView.Margin = new Thickness(0, 10, 0, 0)));

                await Dispatcher.BeginInvoke(new Action(() => (paragraphView as IParagraphEditView).
                    RefreshParagraphsUI = RefreshParagraphsUI));
                await Dispatcher.BeginInvoke(new Action(() => (paragraphView as IParagraphEditView).
                    RefreshParagraphsUIAfterSwap = RefreshParagraphsUIAfterSwap));

                await Dispatcher.BeginInvoke(new Action(() => currentPanel.Children.Add(paragraphView)));
                await Dispatcher.BeginInvoke(new Action(() => currentPanel.UpdateLayout()));
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }

            DoWeHaveToCancelParagraphUploading = false;
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

        private void GoToListMode(object sender, MouseButtonEventArgs e)
        {
            RefreshItemsUI();

            BackToPreviousItemStaticImage.IsEnabled = true;
            AddNewItemStaticImage.IsEnabled = true;

            DocumentTreeView.Visibility = Visibility.Collapsed;
            ListModeScroll.Visibility = Visibility.Visible;

            GoToListModeTextBox.Visibility = Visibility.Collapsed;
            GoToTreeViewModeTextBox.Visibility = Visibility.Visible;
        }

        private void GoToTreeViewMode(object sender, MouseButtonEventArgs e)
        {
            CreateDocumentTreeView(DocController.CurrentDocument);

            BackToPreviousItemStaticImage.IsEnabled = false;
            AddNewItemStaticImage.IsEnabled = false;

            DocumentTreeView.Visibility = Visibility.Visible;
            ListModeScroll.Visibility = Visibility.Collapsed;

            GoToListModeTextBox.Visibility = Visibility.Visible;
            GoToTreeViewModeTextBox.Visibility = Visibility.Collapsed;
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

        /// <summary>
        /// If the text of a search text box is a default text (like a placeholder)
        /// we must clear the textbox for a user's purposes
        /// </summary>
        private void ParagraphSearchTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == ParagraphSearchTextBoxDefaultText)
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
                textBox.Text = ParagraphSearchTextBoxDefaultText;
        }

        /// <summary>
        /// On every text changed we must find all the elements which sutisfiy the search query
        /// </summary>
        private void ParagraphSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ParagraphSearchTextBox == null || DocController.CurrentParagraphsList == null)
                return;

            string searchText = ParagraphSearchTextBox.Text;

            if (string.IsNullOrEmpty(searchText) || searchText == ParagraphSearchTextBoxDefaultText)
            {
                CancelParagraphsUploading();
                UploadParagraphsToParagraphsListPanel(DocController.CurrentParagraphsList);
                return;
            }

            List<Paragraph> suitableParagraphs = new List<Paragraph>();
            List<Paragraph> currentParagraphs = DocController.CurrentParagraphsList;

            foreach (Paragraph paragraph in currentParagraphs)
            {
                if (paragraph.ParagraphElement.Title.IndexOf(searchText) > -1)
                    suitableParagraphs.Add(paragraph);
            }

            CancelParagraphsUploading();
            UploadParagraphsToParagraphsListPanel(suitableParagraphs);
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
        #endregion

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

        private void CreateNewDocument(object sender, MouseButtonEventArgs e)
        {
            List<Document> currentDocumentList = DocController.Documents;
            Documentation documentation = DocController.Documentation;

            if (currentDocumentList == null || documentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала откройти документацию", MessageBoxButton.OK);
                return;
            }

            CreateNewDocumentWindow createNewDocumentWindow = new CreateNewDocumentWindow(currentDocumentList,
                documentation);

            if (createNewDocumentWindow.ShowDialog() == true)
            {
                RefreshDocumentUI();
            }
        }
    }
}
