using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Services;

using SDWP.Interfaces;
using SDWP.Exceptions;
using SDWP.Factories;
using SDWP.Models;
using System.IO;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для DocumentTemplatesPage.xaml
    /// </summary>
    public partial class DocumentTemplatesPage : Page, IAccountPage
    {
        #region IAccountPage
        public Action CloseAccGrid { get; set; }
        #endregion

        #region Services
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }

        private ILocalTemplateService LocalTemplateService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        #region Propeties
        private UserInfo CurrentUser { get; }
        private string DefaultStoragePath { get; } = @"C:\Users\Aero\Desktop\Templates";

        private TextBlock SelectedLocalTemplateTextBlock { get; set; }
        private StackPanel LocalTemplatesStackPanel { get; set; }
        private StackPanel CloudTemplatesStackPanel { get; set; }
        private ListBox LocalTemplatesListBox { get; set; }
        private TreeView TemplateTreeView { get; set; }
        private PageHeader PageHeader { get; set; }
        private TextBox HintTextBox { get; set; }
        #endregion

        #region Constructors and initializing methods
        public DocumentTemplatesPage(UserInfo currentUser)
        {
            InitializeComponent();
            InitializeProperties();
            InitializeServices();
            UploadTemplatesFromLocalStorage();

            PageHeader.OnRefresh = new Action(async () => await UploadTemplatesFromLocalStorage());

            CurrentUser = currentUser;
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            SdwpAbstractFactory = new SdwpAbstractFactory();

            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
            LocalTemplateService = ServiceAbstractFactory.GetLocalTemplateService();
            LocalTemplateService.StoragePath = DefaultStoragePath;
        }

        private void InitializeProperties()
        {
            LocalTemplatesStackPanel = localTemplatesStackPanel;
            CloudTemplatesStackPanel = cloudTemplatesStackPanel;
            LocalTemplatesListBox = localTemplatesListBox;
            TemplateTreeView = templateTreeView;
            PageHeader = pageHeader;
            HintTextBox = hintTextBox;
        }

        private async Task UploadTemplatesFromLocalStorage()
        {
            List<LocalTemplate> templates = (await LocalTemplateService.GetLocalTemplates()).ToList();
            LocalTemplatesListBox.ItemsSource = templates;
        }
        #endregion

        #region Parent setting
        private void SetTemplateItemsParents(LocalTemplate localTemplate)
        {
            if (localTemplate.Template.Items != null)
            {
                foreach (Item item in localTemplate.Template.Items)
                {
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
                    (i as IParentableItem).SetParents(item, item.Items);
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

        #region Templates list boxes events
        private void ListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            selectedItem.IsSelected = true;

            CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as LocalTemplate);
            //SelectedTemplateTextBlock.Text = (selectedItem.DataContext as Documentation).Name;
        }

        private void ListBoxItemMouseEnter(object sender, MouseEventArgs e)
        {
            (sender as ListBoxItem).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        }

        private void ListBoxItemMouseLeave(object sender, MouseEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            if (!listBoxItem.IsSelected)
            {
                listBoxItem.Background = new SolidColorBrush(Colors.LightGray);
            }
        }

        private void TemplateTypesTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Add(TextDecorations.Underline);
        }

        private void TemplateTypesTextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Clear();
        }
        #endregion

        #region Template tree view
        /// <summary>
        /// Buids a tree view with a given template file, also sets parents to all tree nodes (items or paragraphs)
        /// </summary>
        /// <param name="localTemplate"></param>
        private void CreateTemplateTreeView(LocalTemplate localTemplate)
        {
            TemplateTreeView.Items.Clear();
            SetTemplateItemsParents(localTemplate);

            foreach (Item item in localTemplate.Template.Items)
            {
                TemplateTreeViewItem treeViewItem = CreateTemplateTreeViewItem(item);

                UploadItemsToTreeView(item, treeViewItem);

                if (item.Paragraphs != null)
                {
                    foreach (Paragraph paragraph in item.Paragraphs)
                    {
                        TemplateTreeViewItem treeItem = CreateTemplateTreeViewItem(paragraph);
                        treeViewItem.Items.Add(treeItem);
                    }
                }

                TemplateTreeView.Items.Add(treeViewItem);
            }
        }

        /// <summary>
        /// Recursive algorithm which upload items and paragraphs to the tree view root item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rootItem"></param>
        private void UploadItemsToTreeView(Item item, TemplateTreeViewItem rootItem)
        {
            if (item.Items != null)
                foreach (Item i in item.Items)
                {
                    TemplateTreeViewItem treeViewItem = CreateTemplateTreeViewItem(i);

                    rootItem.Items.Add(treeViewItem);

                    if (i.Items != null)
                    {
                        UploadItemsToTreeView(i, treeViewItem);
                    }

                    if (i.Paragraphs != null)
                    {
                        foreach (Paragraph paragraph in i.Paragraphs)
                        {
                            TemplateTreeViewItem treeItem = CreateTemplateTreeViewItem(paragraph);
                            treeViewItem.Items.Add(treeItem);
                        }
                    }
                }
        }

        private TemplateTreeViewItem CreateTemplateTreeViewItem(Paragraph paragraph)
        {
            TemplateTreeViewItem templateTreeViewItem = new TemplateTreeViewParagraphItem(paragraph)
            {
                Style = Resources["treeViewContentItemStyle"] as Style,
                ContextMenu = Resources["treeViewParagraphItemContextMenu"] as ContextMenu,
                Margin = new Thickness(10, 0, 0, 0),
            };

            return templateTreeViewItem;
        }

        private TemplateTreeViewItem CreateTemplateTreeViewItem(Item item)
        {
            TemplateTreeViewItem templateTreeViewItem = new TemplateTreeViewItemItem(item);

            if (item.Paragraphs != null)
            {
                templateTreeViewItem.Style = Resources["treeViewItemsItemStyle"] as Style;
                templateTreeViewItem.ContextMenu = Resources["treeViewContentItemContextMenu"] as ContextMenu;
            }
            else
                templateTreeViewItem.Style = Resources["treeViewItemsItemStyle"] as Style;
            templateTreeView.Margin = new Thickness(10, 0, 0, 0);

            return templateTreeViewItem;
        }

        private void TreeViewItemMouseDown(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = sender as TreeViewItem;
            selectedItem.IsSelected = true;

            if (selectedItem is TemplateTreeViewParagraphItem)
            {
                HintTextBox.Text = (selectedItem as TemplateTreeViewParagraphItem).Paragraph.ParagraphElement.Hint;
            }
            else
            {
                HintTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Enables the edditing of a name of a selected tree view item
        /// </summary>
        private void OnTreeViewRenameItem(object sender, RoutedEventArgs e)
        {
            (TemplateTreeView.SelectedItem as TemplateTreeViewItem).IsEnabledForEdditing = true;
        }

        private void AddNewItem(object sender, RoutedEventArgs e)
        {
            TemplateTreeViewItem templateTreeViewItem = TemplateTreeView.SelectedItem as TemplateTreeViewItem;
            Item selectedItem = (templateTreeViewItem as TemplateTreeViewItemItem).Item;

            CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(selectedItem.Items, selectedItem);

            if (createNewItemWindow.ShowDialog() == true)
            {
                CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as LocalTemplate);
            }
        }

        private void AddNewParagraph(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (TemplateTreeView.SelectedItem as TemplateTreeViewItemItem).Item;
            CreateTemplateTreeViewParagraphWindow createWindow = new CreateTemplateTreeViewParagraphWindow(selectedItem);

            if (createWindow.ShowDialog() == true)
            {
                CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as LocalTemplate);
            }
        }

        private void TreeViewItemTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            (TemplateTreeView.SelectedItem as TemplateTreeViewItem).IsEnabledForEdditing = false;
        }

        private void OnTreeViewDeleteItem(object sender, RoutedEventArgs e)
        {
            TemplateTreeViewItem selectedItem = TemplateTreeView.SelectedItem as TemplateTreeViewItem;
            if (selectedItem is TemplateTreeViewItemItem)
            {
                Item item = (selectedItem as TemplateTreeViewItemItem).Item;
                item.ParentList.Remove(item);
            }
            else
            {
                Paragraph paragraph = (selectedItem as TemplateTreeViewParagraphItem).Paragraph;
                paragraph.ParentList.Remove(paragraph);
            }

            CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as LocalTemplate);
        }

        private void AddNewItemToRoot(object sender, RoutedEventArgs e)
        {
            LocalTemplate currentLocalTemplate = LocalTemplatesListBox.SelectedItem as LocalTemplate;
            CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(currentLocalTemplate.Template.Items, null);

            if (createNewItemWindow.ShowDialog() == true)
            {
                CreateTemplateTreeView(currentLocalTemplate);
            }
        }
        #endregion

        #region Template buttons (4 top buttons) methods
        /// <summary>
        /// Saves all templates to the local templates folder
        /// </summary>
        private async void SaveAllTemplatesBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                IEnumerable<LocalTemplate> templates = LocalTemplatesListBox.ItemsSource as IEnumerable<LocalTemplate>;
                foreach (LocalTemplate template in templates)
                {
                    await LocalTemplateService.RewriteTemplateFile(template);
                }
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            finally
            {
                PageHeader.SwitchOffTheLoader();
            }
        }
        /// <summary>
        /// Saves current template to a file in a local templates folder
        /// </summary>
        private async void SaveCurrentTemplate(object sender, RoutedEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                LocalTemplate template = LocalTemplatesListBox.SelectedItem as LocalTemplate;
                await LocalTemplateService.RewriteTemplateFile(template);
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            finally
            {
                PageHeader.SwitchOffTheLoader();
            }
        }
        /// <summary>
        /// Deletes the template from the list box, deletes the template file from
        /// the template directory and also clears the tree view 
        /// </summary>
        private void DeleteTemplate(object sender, RoutedEventArgs e)
        {
            if (LocalTemplatesListBox.SelectedItem is LocalTemplate selectedTemplate)
            {
                try
                {
                    LocalTemplateService.DeleteTemplateFile(selectedTemplate);

                    List<LocalTemplate> listBoxItemsSource = LocalTemplatesListBox.ItemsSource as List<LocalTemplate>;
                    listBoxItemsSource.Remove(selectedTemplate);
                    LocalTemplatesListBox.ItemsSource = null;
                    LocalTemplatesListBox.ItemsSource = listBoxItemsSource;

                    TemplateTreeView.Items.Clear();
                }
                catch (IOException ex)
                {
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleWithMessageBox(ex);
                }
            }
        }
        /// <summary>
        /// Creates a new template in a list box and also creates a file of this template in a template folder
        /// </summary>
        private async void CreateNewTemplate(object sender, RoutedEventArgs e)
        {
            try
            {
                LocalTemplate template = new LocalTemplate(new Template()
                {
                    Items = new List<Item>(),
                    TemplateName = "Новый шаблон"
                });

                await LocalTemplateService.CreateTemplateFile(template);

                List<LocalTemplate> templates = (LocalTemplatesListBox.ItemsSource as IEnumerable<LocalTemplate>).ToList();
                templates.Add(template);

                LocalTemplatesListBox.ItemsSource = templates;
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }
        #endregion

        private void GoToLocalTemplatesMode(object sender, MouseButtonEventArgs e)
        {
            LocalTemplatesStackPanel.Visibility = Visibility.Visible;
            CloudTemplatesStackPanel.Visibility = Visibility.Collapsed;
        }

        private void GoToCloudTemplatesMode(object sender, MouseButtonEventArgs e)
        {
            LocalTemplatesStackPanel.Visibility = Visibility.Collapsed;
            CloudTemplatesStackPanel.Visibility = Visibility.Visible;
        }

        private void HintTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TreeViewItem selectedTreeViewItem = TemplateTreeView.SelectedItem as TreeViewItem;
            if (selectedTreeViewItem is TemplateTreeViewParagraphItem)
            {
                Paragraph paragraph = (selectedTreeViewItem as TemplateTreeViewParagraphItem).Paragraph;
                paragraph.ParagraphElement.Hint = (sender as TextBox).Text;
            }
        }
    }
}
