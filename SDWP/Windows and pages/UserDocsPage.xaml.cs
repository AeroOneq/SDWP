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
using System.Data.SqlClient;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Services;
using ApplicationLib.Database;

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

        #region Services and factories
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }

        private IExceptionHandler ExceptionHandler { get; set; }
        private ICloudDocumentationService CloudDocumentationService { get; set; }
        private ICloudDocumentsService CloudDocumentsService { get; set; }
        private ILocalDocumentationService LocalDocumentationService { get; set; }
        #endregion

        #region Propeties
        private PageHeader PageHeader { get; }
        private string CurrentFilePath { get; set; }

        private List<LocalDocumentation> LocalDocumentations { get; set; }
        private List<Documentation> CloudDocumentation { get; set; }

        private MainPage MainPage { get; }
        private StackPanel LocalDocsPanel { get; set; }
        private StackPanel CloudDocsPanel { get; set; }
        private TextBlock GoToLocalDocsTextBlock { get; set; }
        private TextBlock GoToCloudDocsTextBlock { get; set; }
        private TextBlock SelectedLocalDocumentationTextBlock { get; set; }
        private StackPanel LocalDocumentationPropertiesStackPanel { get; set; }
        private StackPanel CloudDocumentationPropertiesStackPanel { get; set; }
        private TextBox FilePathTextBox { get; set; }
        private ListBox LocalDocumentationListBox { get; set; }
        private ListBox CloudDocumentationListBox { get; set; }
        #endregion

        #region Constructors and initializing methods
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
            LocalDocumentationPropertiesStackPanel = localDocumentationPropertiesStackPanel;
            CloudDocumentationPropertiesStackPanel = cloudDocumentationPropertiesStackPanel;
            FilePathTextBox = filePathTextBox;
            LocalDocumentationListBox = offlineDocumentationListBox;
            CloudDocumentationListBox = onlineDocumentationListBox;
        }

        private void InitializeServices()
        {
            SdwpAbstractFactory = new SdwpAbstractFactory();
            ServiceAbstractFactory = new ServiceAbstractFactory();

            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
            CloudDocumentationService = ServiceAbstractFactory.GetCloudDocumentationService(
                DatabaseProperties.ConnectionString);
            LocalDocumentationService = ServiceAbstractFactory.GetLocalDocumentationService();
            CloudDocumentsService = ServiceAbstractFactory.GetCloudDocumentsService(DatabaseProperties.ConnectionString);
        }
        #endregion

        #region Upload documentation methods
        private async Task UploadDocumentationsFromLocalSotrage()
        {
            try
            {
                LocalDocumentations = (await LocalDocumentationService.GetLocalDocumentations()).ToList();
                LocalDocumentationListBox.ItemsSource = LocalDocumentations.Select(lc => lc.Documentation).ToList();
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

        private async Task UploadDocumentationsFromCloudSotrage()
        {
            try
            {
                CloudDocumentation = (await CloudDocumentationService.GetDocumentations("AuthorID", UserInfo.CurrentUser.ID))
                    .ToList();
                CloudDocumentationListBox.ItemsSource = CloudDocumentation;
            }
            catch (SqlException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
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

        private async void GoToLocalDocumentationPanel(object sender, RoutedEventArgs e)
        {
            GoToLocalDocsTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
            GoToCloudDocsTextBlock.Foreground = new SolidColorBrush(Colors.Black);

            LocalDocsPanel.Visibility = Visibility.Visible;
            CloudDocsPanel.Visibility = Visibility.Collapsed;
            CloudDocumentationPropertiesStackPanel.Visibility = Visibility.Collapsed;
            LocalDocumentationPropertiesStackPanel.Visibility = Visibility.Visible;

            if (LocalDocumentationService.StoragePath != null)
                await UploadDocumentationsFromLocalSotrage();
        }

        private async void GoToCloudDocumentationPanel(object sender, RoutedEventArgs e)
        {
            GoToLocalDocsTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            GoToCloudDocsTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);

            LocalDocsPanel.Visibility = Visibility.Collapsed;
            CloudDocsPanel.Visibility = Visibility.Visible;
            CloudDocumentationPropertiesStackPanel.Visibility = Visibility.Visible;
            LocalDocumentationPropertiesStackPanel.Visibility = Visibility.Collapsed;

            await UploadDocumentationsFromCloudSotrage();
        }
        #endregion

        #region Cloud documentation methods
        /// <summary>
        /// Deletes the documentation from the database and also deletes all the docuemnts which were in
        /// that docuemntation
        /// </summary>
        private async void DeleteCloudDocumentation(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CloudDocumentationListBox.SelectedItem is Documentation selectedDocumentation)
                {
                    await CloudDocumentationService.DeleteDocumentation(selectedDocumentation);
                    await DeleteAllDocumentationDocuments(selectedDocumentation.ID);

                    await UploadDocumentationsFromCloudSotrage();
                    SDWPMessageBox.ShowSDWPMessageBox("Успех", "Документация и все документы связанные с ней успешно удалены",
                        MessageBoxButton.OK);
                }
            }
            catch (SqlException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private async Task DeleteAllDocumentationDocuments(int documentationID)
        {
            IEnumerable<Document> documents = await CloudDocumentsService.GetDocuments("DocumentationID", documentationID);
            foreach (Document document in documents)
            {
                await CloudDocumentsService.DeleteDocument(document);
            }
        }

        private async void CreateCloudDocumentation(object sender, RoutedEventArgs e)
        {
            CreateNewDocumentationWindow createNewDocumentationWindow =
                new CreateNewDocumentationWindow(CloudDocumentationService);

            if (createNewDocumentationWindow.ShowDialog() == true)
            {
                await UploadDocumentationsFromCloudSotrage();
            }
        }

        private async void UploadCloudDocumentationToMainPage(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CloudDocumentationListBox.SelectedItem is Documentation selectedDocumentation)
                {
                    List<Document> documents = (await CloudDocumentsService.GetDocuments("DocumentationID",
                        selectedDocumentation.ID)).ToList();

                    MainPage.UploadCloudDocumentation(selectedDocumentation, documents);
                    CloseAccGrid();
                }
                else
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете докуемнтацию для открытия",
                        MessageBoxButton.OK);
                }
            }
            catch (SqlException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        /// <summary>
        /// Creates a local copy of a selected cloud documentation
        /// </summary>
        private async void CreateLocalCopy(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CloudDocumentationListBox.SelectedItem is Documentation selectedDocumentation)
                {
                    List<Document> documents = (await CloudDocumentsService.GetDocuments(
                        "DocumentationID", selectedDocumentation.ID)).ToList();
                    selectedDocumentation.StorageType = StorageType.Local;

                    string filePath = GetFilePathToLocalCopy();

                    if (filePath != null)
                    {
                        LocalDocumentation localDocumentation = new LocalDocumentation(
                            selectedDocumentation, documents)
                        {
                            DocumentationPath = filePath
                        };

                        await LocalDocumentationService.CreateLocalDocumentationFile(localDocumentation);
                    }
                    else
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы не выбрали путь для сохранения",
                            MessageBoxButton.OK);
                    }
                }
                else
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете документацию из облака",
                        MessageBoxButton.OK);
                }
            }
            catch (SqlException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
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
        /// Gets a file path via save file dialog for a copy of a cloud documentation
        /// </summary>
        private string GetFilePathToLocalCopy()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Выберете место для создание копии",
                FileName = "Новый документ",
                AddExtension = true,
                Filter = "(*sdwp)|*sdwp",
                OverwritePrompt = true,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                if (filePath.Substring(filePath.Length - LocalDocumentationService.Extension.Length) !=
                    LocalDocumentationService.Extension)
                {
                    filePath += LocalDocumentationService.Extension;
                }

                return filePath;
            }

            return null;
        }
        #endregion

        #region Local documentation methods
        private async void CreateLocalDocumentation(object sender, RoutedEventArgs e)
        {
            if (LocalDocumentationService.StoragePath == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала откройте папку с документами", MessageBoxButton.OK);
                return;
            }

            CreateNewDocumentationWindow createNewDocumentationWindow =
                new CreateNewDocumentationWindow(LocalDocumentationService);

            if (createNewDocumentationWindow.ShowDialog() == true)
            {
                await UploadDocumentationsFromLocalSotrage();
            }
        }

        /// <summary>
        /// Publishes selected documentation to the database with a private access 
        /// </summary>
        private async void PublishLocalDocumentation(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LocalDocumentationListBox.SelectedItem is Documentation selectedDocumentation)
                {
                    SetDocumentationPropertiesForPublishing(selectedDocumentation);

                    List<Document> documents = LocalDocumentations.Find(ld => ld.Documentation == selectedDocumentation).Documents;

                    await CloudDocumentationService.InsertDocumentation(selectedDocumentation);
                    await PublishDocuments(await GetLastDocumentationID(), documents);

                    SDWPMessageBox.ShowSDWPMessageBox("Успех", "Документация успешно опубликована", MessageBoxButton.OK);
                }
                else
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Сначала выберете докуемнтацию для публикации", MessageBoxButton.OK);
                }
            }
            catch (SqlException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private async Task PublishDocuments(int documentationID, List<Document> documents)
        {
            foreach (Document document in documents)
            {
                document.DocumentationID = documentationID;
                await CloudDocumentsService.InsertDocument(document);
            }
        }

        private void SetDocumentationPropertiesForPublishing(Documentation documentation)
        {
            documentation.Access = Access.Private;
            documentation.StorageType = StorageType.Cloud;
        }

        /// <summary>
        /// Gets the ID of a last user's (which is logged in the system) documentation. 
        /// </summary>
        private async Task<int> GetLastDocumentationID()
        {
            List<Documentation> documentations = (await CloudDocumentationService.GetDocumentations("AuthorID",
                UserInfo.CurrentUser.ID)).ToList();

            return documentations[documentations.Count - 1].ID;
        }

        private void UploadLocalDocumentationToMainPage(object sender, RoutedEventArgs e)
        {
            if (!(LocalDocumentationListBox.SelectedItem is Documentation selectedDocumentation))
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы не выбрали документацию",
                    MessageBoxButton.OK);
                return;
            }

            LocalDocumentation localDocumentation = LocalDocumentations.Find(
                ld => ld.Documentation == selectedDocumentation);
            localDocumentation.DocumentationPath = Path.Combine(CurrentFilePath,
                localDocumentation.Documentation.Name + ".sdwp");

            MainPage.UploadLocalDocumentation(localDocumentation);
            CloseAccGrid();
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

        private async void DeleteLocalDocumentation(object sender, RoutedEventArgs e)
        {
            if (!(LocalDocumentationListBox.SelectedItem is Documentation selectedDocumentation))
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Вы не выбрали документацию", MessageBoxButton.OK);
                return;
            }

            LocalDocumentationService.DeleteLocalDocumentationFile(LocalDocumentations.Find((
                ld => ld.Documentation == selectedDocumentation)));

            await UploadDocumentationsFromLocalSotrage();
        }
        #endregion
    }
}
