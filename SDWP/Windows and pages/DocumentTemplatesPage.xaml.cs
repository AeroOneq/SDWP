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
using ApplicationLib.Database;
using ApplicationLib.Factories;

using SDWP.Interfaces;
using SDWP.Exceptions;
using SDWP.Factories;
using SDWP.Models;
using System.IO;
using System.Data.SqlClient;

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

        #region Services and factories
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }

        private ILocalTemplateService LocalTemplateService { get; set; }
        private ICloudTemplateService CloudTemplateService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        #region Propeties
        private string DefaultStoragePath { get; }
        //status: either local templates or cloud templates
        private bool LocalTemplatesMode { get; set; }

        //Framework elements
        private TextBlock SelectedLocalTemplateTextBlock { get; set; }
        private StackPanel LocalTemplatesStackPanel { get; set; }
        private StackPanel CloudTemplatesStackPanel { get; set; }
        private ListBox LocalTemplatesListBox { get; set; }
        private ListBox CloudTemplatesListBox { get; set; }
        private TreeView TemplateTreeView { get; set; }
        private PageHeader PageHeader { get; set; }
        private TextBox TemplateNameTextBox { get; set; }
        private TextBox HintTextBox { get; set; }
        private TextBlock LocalTemplatesTextBlock { get; set; }
        private TextBlock CloudTemplatesTextBlock { get; set; }
        #endregion

        #region Constructors and initializing methods
        public DocumentTemplatesPage(UserInfo currentUser)
        {
            InitializeComponent();
            InitializeProperties();
            InitializeServices();

            LocalTemplateService.StoragePath = DefaultStoragePath = 
                Path.Combine(Directory.GetCurrentDirectory(), "Templates");
            GoToLocalTemplatesMode(null, null);
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            SdwpAbstractFactory = new SdwpAbstractFactory();

            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
            LocalTemplateService = ServiceAbstractFactory.GetLocalTemplateService();
            LocalTemplateService.StoragePath = DefaultStoragePath;
            CloudTemplateService = ServiceAbstractFactory.GetCloudTemplateService();
        }

        private void InitializeProperties()
        {
            LocalTemplatesStackPanel = localTemplatesStackPanel;
            CloudTemplatesStackPanel = cloudTemplatesStackPanel;
            LocalTemplatesListBox = localTemplatesListBox;
            CloudTemplatesListBox = cloudTemplatesListBox;
            TemplateTreeView = templateTreeView;
            PageHeader = pageHeader;
            TemplateNameTextBox = templateNameTextBox;
            HintTextBox = hintTextBox;
            LocalTemplatesTextBlock = goToLocalDocumentation;
            CloudTemplatesTextBlock = goToCloudDocumentation;
        }
        #endregion

        #region Templates uploading
        private async Task UploadTemplatesFromLocalStorage()
        {
            PageHeader.SwitchOnTopLoader();

            try
            {
                List<LocalTemplate> localTemplates = (await LocalTemplateService.GetLocalTemplates()).ToList();
                LocalTemplatesListBox.ItemsSource = localTemplates;

                PageHeader.SwitchOffTheLoader();
            }
            catch (IOException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private async Task UploadTemplatesFromCloudStorage()
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                List<Template> templates = (await CloudTemplateService.GetUserTemplates(UserInfo.CurrentUser.ID)).ToList();
                CloudTemplatesListBox.ItemsSource = templates;

                PageHeader.SwitchOffTheLoader();
            }
            catch (SqlException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }
        #endregion

        #region Parent setting
        private void SetTemplateItemsParents(Template template)
        {
            if (template.Items != null)
            {
                foreach (Item item in template.Items)
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

        private void ListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            selectedItem.IsSelected = true;

            Template selectedTemplate = GetSelectedTemplate();
            TemplateNameTextBox.Text = selectedTemplate.TemplateName;

            CreateTemplateTreeView(selectedTemplate);
        }
        #endregion

        #region Template tree view
        /// <summary>
        /// Buids a tree view with a given template file, also sets parents to all tree nodes (items or paragraphs)
        /// </summary>
        /// <param name="localTemplate"></param>
        private void CreateTemplateTreeView(Template template)
        {
            try
            {
                TemplateTreeView.Items.Clear();
                SetTemplateItemsParents(template);

                foreach (Item item in template.Items)
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
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
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
                HintTextBox.Text = (selectedItem as TemplateTreeViewParagraphItem).
                    Paragraph.ParagraphElement.Hint;
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
            try
            {
                if (TemplateTreeView.SelectedItem is TemplateTreeViewItem templateTreeViewItem)
                {
                    Item selectedItem = (templateTreeViewItem as TemplateTreeViewItemItem).Item;

                    CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(selectedItem.Items, selectedItem);

                    if (createNewItemWindow.ShowDialog() == true)
                    {
                        RefreshTemplateTreeView();
                    }
                }
                else
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете шаблон",
                        MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private void RefreshTemplateTreeView()
        {
            if (LocalTemplatesMode)
                CreateTemplateTreeView((LocalTemplatesListBox.SelectedItem as LocalTemplate).Template);
            else
                CreateTemplateTreeView(CloudTemplatesListBox.SelectedItem as Template);
        }

        private void AddNewParagraph(object sender, RoutedEventArgs e)
        {
            try
            {
                Item selectedItem = (TemplateTreeView.SelectedItem as TemplateTreeViewItemItem).Item;
                CreateTemplateTreeViewParagraphWindow createWindow =
                    new CreateTemplateTreeViewParagraphWindow(selectedItem);

                if (createWindow.ShowDialog() == true)
                {
                    RefreshTemplateTreeView();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private void TreeViewItemTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            (TemplateTreeView.SelectedItem as TemplateTreeViewItem).IsEnabledForEdditing = false;
        }

        private void OnTreeViewDeleteItem(object sender, RoutedEventArgs e)
        {
            try
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

                RefreshTemplateTreeView();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private void AddNewItemToRoot(object sender, RoutedEventArgs e)
        {
            try
            {
                Template selectedTemplate = GetSelectedTemplate();
                CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(selectedTemplate.Items, null);

                if (createNewItemWindow.ShowDialog() == true)
                {
                    RefreshTemplateTreeView();
                }
            }
            catch (NullReferenceException)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете шаблон",
                    MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        /// <summary>
        /// Gets a selected template from a LocalTempaltesListBox if the current mode is local mode
        /// or from a CloudTemplatesListBox if the current mode is cloud mode
        /// </summary>
        private Template GetSelectedTemplate()
        {
            if (LocalTemplatesMode)
                return (LocalTemplatesListBox.SelectedItem as LocalTemplate).Template;
            else
                return CloudTemplatesListBox.SelectedItem as Template;
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

                if (LocalTemplatesMode)
                {
                    if (LocalTemplatesListBox.ItemsSource is IEnumerable<LocalTemplate> templates)
                    {
                        foreach (LocalTemplate template in templates)
                            await LocalTemplateService.RewriteTemplateFile(template);

                        LocalTemplatesListBox.ItemsSource = null;
                        await UploadTemplatesFromLocalStorage();
                    }
                    else
                    {
                        PageHeader.SwitchOffTheLoader();
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала загрузите шаблоны",
                            MessageBoxButton.OK);

                        return;
                    }
                }
                else
                {
                    IEnumerable<Template> templates = CloudTemplatesListBox.ItemsSource as IEnumerable<Template>;

                    if (CloudTemplatesListBox.ItemsSource is IEnumerable<Template>)
                    {
                        foreach (Template template in templates)
                            await CloudTemplateService.UpdateTemplate(template);

                        CloudTemplatesListBox.ItemsSource = null;
                        await UploadTemplatesFromCloudStorage();
                    }
                    else
                    {
                        PageHeader.SwitchOffTheLoader();
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала загрузите шаблоны",
                            MessageBoxButton.OK);

                        return;
                    }
                }

                PageHeader.SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Успех", "Все шаблоны выбранного раздела сохранены",
                     MessageBoxButton.OK);
            }
            catch (IOException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
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

                if (LocalTemplatesMode)
                {
                    if (LocalTemplatesListBox.SelectedItem is LocalTemplate localTemplate)
                    {
                        await LocalTemplateService.RewriteTemplateFile(localTemplate);

                        LocalTemplatesListBox.ItemsSource = null;
                        await UploadTemplatesFromLocalStorage();
                    }
                    else
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Выберете шаблон для сохранения",
                            MessageBoxButton.OK);
                        return;
                    }
                }
                else
                {
                    if (CloudTemplatesListBox.SelectedItem is Template template)
                    {
                        await CloudTemplateService.UpdateTemplate(template);

                        CloudTemplatesListBox.ItemsSource = null;
                        await UploadTemplatesFromCloudStorage();
                    }
                    else
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Выберете шаблон для сохранения",
                            MessageBoxButton.OK);
                        return;
                    }
                }

                PageHeader.SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Успех", "Шаблон успешно сохранен", MessageBoxButton.OK);
            }
            catch (IOException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
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
        private async void DeleteTemplate(object sender, RoutedEventArgs e)
        {
            if (SDWPMessageBox.ConfirmAction() == MessageBoxResult.Cancel)
                return;

            try
            {
                if (LocalTemplatesMode)
                {
                    if (LocalTemplatesListBox.SelectedItem is LocalTemplate selectedTemplate)
                    {
                        LocalTemplateService.DeleteTemplateFile(selectedTemplate);

                        await UploadTemplatesFromLocalStorage();
                        LocalTemplatesListBox.ItemsSource = null;
                    }
                    else
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете шаблон для удаления",
                            MessageBoxButton.OK);
                        return;
                    }
                }
                else
                {
                    if (CloudTemplatesListBox.SelectedItem is Template selectedTemplate)
                    {
                        await CloudTemplateService.DeleteTemplate(selectedTemplate);

                        CloudTemplatesListBox.ItemsSource = null;
                        await UploadTemplatesFromCloudStorage();
                    }
                    else
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете шаблон для удаления",
                            MessageBoxButton.OK);
                        return;
                    }
                }

                TemplateTreeView.Items.Clear();
                PageHeader.SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Успех", "Шаблон успешно удален", MessageBoxButton.OK);
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

        /// <summary>
        /// Creates a new template in a list box and also creates a file of this template in a template folder
        /// </summary>
        private async void CreateNewTemplate(object sender, RoutedEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                Template template = CreateNewTemplate(); 
                if (LocalTemplatesMode)
                {
                    LocalTemplate localTemplate = new LocalTemplate(template);

                    await LocalTemplateService.CreateTemplateFile(localTemplate);

                    LocalTemplatesListBox.ItemsSource = null;
                    await UploadTemplatesFromLocalStorage();
                }
                else
                {
                    await CloudTemplateService.InsertTemplate(template);

                    CloudTemplatesListBox.ItemsSource = null;
                    await UploadTemplatesFromCloudStorage();
                }

                PageHeader.SwitchOffTheLoader();
                SDWPMessageBox.ShowSDWPMessageBox("Успех", "Шаблон успешно создан", MessageBoxButton.OK);
            }
            catch (IOException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private Template CreateNewTemplate()
        {
            return new Template()
            {
                Items = new List<Item>(),
                TemplateName = "Новый шаблон",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserID = UserInfo.CurrentUser.ID
            };
        }
        #endregion

        #region Mode switching methods
        private async void GoToLocalTemplatesMode(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                LocalTemplatesStackPanel.Visibility = Visibility.Visible;
                CloudTemplatesStackPanel.Visibility = Visibility.Collapsed;

                LocalTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
                CloudTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.Black);

                if (LocalTemplateService.StoragePath != null)
                    await UploadTemplatesFromLocalStorage();

                LocalTemplatesMode = true;
                PageHeader.SwitchOffTheLoader();
            }
            catch (IOException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private async void GoToCloudTemplatesMode(object sender, MouseButtonEventArgs e)
        {
            PageHeader.SwitchOnTopLoader();

            try
            {
                LocalTemplatesStackPanel.Visibility = Visibility.Collapsed;
                CloudTemplatesStackPanel.Visibility = Visibility.Visible;

                LocalTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                CloudTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);

                await UploadTemplatesFromCloudStorage();
                LocalTemplatesMode = false;

                PageHeader.SwitchOffTheLoader();
            }
            catch (SqlException ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                PageHeader.SwitchOffTheLoader();
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }
        #endregion

        /// <summary>
        /// If paragraph element is selected the hint is displayed in the Hint text box
        /// If the hint text box's text is changed then the hint of the selected paragraph is changed
        /// to the new value
        /// </summary>
        private void HintTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TreeViewItem selectedTreeViewItem = TemplateTreeView.SelectedItem as TreeViewItem;
            if (selectedTreeViewItem is TemplateTreeViewParagraphItem)
            {
                Paragraph paragraph = (selectedTreeViewItem as TemplateTreeViewParagraphItem).Paragraph;
                paragraph.ParagraphElement.Hint = (sender as TextBox).Text;
            }
        }

        /// <summary>
        /// When the template is selected it's name is displayed in the Template text box.
        /// If the text in the template text box is changed the name of the template is changed
        /// </summary>
        private void TemplateTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (LocalTemplatesMode)
            {
                if (LocalTemplatesListBox.SelectedItem is LocalTemplate selectedLocalTemplate)
                {
                    selectedLocalTemplate.Template.TemplateName = TemplateNameTextBox.Text;
                }
            }
            else
            {
                if (CloudTemplatesListBox.SelectedItem is Template selectedTemplate)
                {
                    selectedTemplate.TemplateName = TemplateNameTextBox.Text;
                }
            }
        }
    }
}
