using System;
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

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Services;

using SDWP.Interfaces;
using SDWP.Exceptions;
using SDWP.Factories;
using SDWP.Models;

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
        #endregion
        public DocumentTemplatesPage(UserInfo currentUser)
        {
            InitializeComponent();
            InitializeProperties();
            InitializeServices();

            UploadTemplatesFromLocalStorage();
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
        }

        private async Task UploadTemplatesFromLocalStorage()
        {
            List<Template> templates = (await LocalTemplateService.GetLocalTemplates()).ToList();
            LocalTemplatesListBox.ItemsSource = templates;
        }

        #region List box
        private void ListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            selectedItem.IsSelected = true;

            CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as Template);
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
        #endregion

        private async void CreateNewTemplate(object sender, RoutedEventArgs e)
        {
            Template template = new Template()
            {
                Items = new List<Item>(),
                TemplateName = "Новый шаблон"
            };

            await LocalTemplateService.CreateTemplateFile(template);
            await UploadTemplatesFromLocalStorage();
        }

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

        #region Template tree view
        private void CreateTemplateTreeView(Template template)
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
                            rootItem.Items.Add(treeItem);
                        }
                    }
                }
        }
        private TemplateTreeViewItem CreateTemplateTreeViewItem(Paragraph paragraph)
        {
            TemplateTreeViewItem templateTreeViewItem = new TemplateTreeViewParagraphItem(paragraph)
            {
                Style = Resources["treeViewContentItemStyle"] as Style,
                Margin = new Thickness(10, 0, 0, 0)
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
        #endregion

        private void TreeViewItemMouseDown(object sender, RoutedEventArgs e)
        {
            (sender as TreeViewItem).IsSelected = true;
        }

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
                CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as Template);
            }
        }

        private void AddNewParagraph(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (TemplateTreeView.SelectedItem as TemplateTreeViewItemItem).Item;
            CreateTemplateTreeViewParagraphWindow createWindow = new CreateTemplateTreeViewParagraphWindow(selectedItem);

            if (createWindow.ShowDialog() == true)
            {
                CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as Template);
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

            CreateTemplateTreeView(LocalTemplatesListBox.SelectedItem as Template);
        }

        private void AddNewItemToRoot(object sender, RoutedEventArgs e)
        {
            Template currentTemplate = LocalTemplatesListBox.SelectedItem as Template;
            CreateNewItemWindow createNewItemWindow = new CreateNewItemWindow(currentTemplate.Items, null);

            if (createNewItemWindow.ShowDialog() == true)
            {
                CreateTemplateTreeView(currentTemplate);
            }
        }
    }
}
