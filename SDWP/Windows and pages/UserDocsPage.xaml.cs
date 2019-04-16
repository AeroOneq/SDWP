using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.IO;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Services;

using SDWP.Models;
using SDWP.Factories;
using SDWP.Interfaces;

using Microsoft.Win32;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для UserDocsPage.xaml
    /// </summary>
    public partial class UserDocsPage : Page, IAccountPage
    {
        #region IAccountPage properties
        public Action CloseAccGrid { get; set; }
        #endregion

        #region Propeties
        private PageHeader PageHeader { get; }
        private string CurrentFilePath { get; set; }

        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ILocalDocumentationService LocalDocumentationService { get; set; }

        private List<LocalDocumentation> LocalDocumentations { get; set; }
        private MainPage MainPage { get; }
        private StackPanel LocalDocsPanel { get; set; }
        private StackPanel CloudDocsPanel { get; set; }
        private TextBlock GoToLocalDocsTextBlock { get; set; } 
        private TextBlock GoToCloudDocsTextBlock { get; set; }
        private TextBlock SelectedLocalDocumentationTextBlock { get; set; }
        private TextBox FilePathTextBox { get; set; }
        private ListBox LocalDocumentationListBox { get; set; }
        private ListBox CloudDocumentationListBox { get; set; }
        #endregion

        public UserDocsPage(MainPage mainPage)
        {
            InitializeComponent();
            InitializeServices();
            InitializeProperties();

            MainPage = mainPage;

            PageHeader = pageHeader;
        }

        private void InitializeProperties()
        {
            LocalDocsPanel = localDocStackPanel;
            CloudDocsPanel = cloudDocStackPanel;
            GoToCloudDocsTextBlock = goToCloudDocumentation;
            GoToLocalDocsTextBlock = goToLocalDocumentation;
            SelectedLocalDocumentationTextBlock = selectedLocalDocumentationNameTextBlock;
            FilePathTextBox = filePathTextBox;
            LocalDocumentationListBox = offlineDocumentationListBox;
            CloudDocumentationListBox = onlineDocumentationListBox;
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            LocalDocumentationService = ServiceAbstractFactory.GetLocalDocumentationService();
        }

        #region Upload documentation methods
        private async Task UploadDocumentationsFromLocalSotrage()
        {
            LocalDocumentations = (await LocalDocumentationService.GetLocalDocumentations()).ToList();
            offlineDocumentationListBox.ItemsSource = LocalDocumentations.Select(lc => lc.Documentation).ToList();
        }
        #endregion

        #region Event handlers
        private void UserDocsBtnMouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.White);
            button.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }

        private void UserDocsBtnMouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.OrangeRed);
            button.Foreground = new SolidColorBrush(Colors.White);
        }
        #endregion

        private void UploadDocumentationToMainPage(object sender, RoutedEventArgs e)
        {
            Documentation selectedDocumentation = offlineDocumentationListBox.SelectedItem as Documentation;
            if (selectedDocumentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы не выбрали документацию", MessageBoxButton.OK);
                return;
            }

            LocalDocumentation localDocumentation = LocalDocumentations.Find(ld => ld.  Documentation == selectedDocumentation);
            localDocumentation.DocumentationPath = Path.Combine(CurrentFilePath, localDocumentation.Documentation.Name + ".sdwp");

            MainPage.UploadLocalDocumentation(localDocumentation);
            CloseAccGrid(); 
        }

        private void ListBoxItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            selectedItem.IsSelected = true;
            SelectedLocalDocumentationTextBlock.Text = (selectedItem.DataContext as Documentation).Name;
        }

        private void DocTypesTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Add(TextDecorations.Underline);
        }

        private void DocTypesTextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Clear();
        }

        private void GoToLocalDocumentationPanel(object sender, RoutedEventArgs e)
        {
            GoToLocalDocsTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
            GoToCloudDocsTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            LocalDocsPanel.Visibility = Visibility.Visible;
            CloudDocsPanel.Visibility = Visibility.Collapsed;
        }

        private void GoToCloudDocumentationPanel(object sender, RoutedEventArgs e)
        {
            GoToLocalDocsTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            GoToCloudDocsTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
            LocalDocsPanel.Visibility = Visibility.Collapsed;
            CloudDocsPanel.Visibility = Visibility.Visible;
        }

        private async void SelectLocalDocumentationFolder(object sender, RoutedEventArgs e)
        {
            string folderPath = FolderDialog.ShowDialog();
            FilePathTextBox.Text = CurrentFilePath = folderPath;
            if (folderPath != null)
            {
                LocalDocumentationService.StoragePath = folderPath;
                await UploadDocumentationsFromLocalSotrage();
            }
        }

        private async void DeleteDocumentation(object sender, RoutedEventArgs e)
        {
            Documentation selectedDocumentation = LocalDocumentationListBox.SelectedItem as Documentation;
            if (selectedDocumentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы не выбрали документацию", MessageBoxButton.OK);
                return;     
            }

            LocalDocumentationService.DeleteLocalDocumentationFile(LocalDocumentations.Find((
                ld => ld.Documentation == selectedDocumentation)));

            await UploadDocumentationsFromLocalSotrage();
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
    }
}
