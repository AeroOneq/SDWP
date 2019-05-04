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
using System.Windows.Shapes;
using System.IO;
using System.Data.SqlClient;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Services;
using ApplicationLib.Database;
using ApplicationLib.Word;

using SDWP.Interfaces;
using SDWP.Exceptions;
using SDWP.Factories;

using Microsoft.Win32;
using ApplicationLib.Factories;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для ExportDocumentationPage.xaml
    /// </summary>
    public partial class ExportDocumentationPage : Page, IAccountPage
    {
        #region IAccountPage properties
        public Action CloseAccGrid { get; set; }
        #endregion

        #region Services and factories
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }
        private IRenderersAbstractFactory RenderersAbstractFactory { get; set; }

        private ICloudDocumentationService CloudDocumentationService { get; set; }
        private ICloudDocumentsService CloudDocumentsService { get; set; }
        private ILocalDocumentationService LocalDocumentationService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        private IWordRenderer WordDocumentRenderer { get; set; }
        #endregion

        #region Properties
        private MainPage MainPage { get; }
        private RenderSettings RenderSettings { get; set; }

        private TextBox WordFilePathTextBox { get; set; }
        private PageHeader PageHeader { get; set; }
        #endregion

        public ExportDocumentationPage(MainPage mainPage)
        {
            InitializeComponent();
            InitializeServices();
            SetUpInitialWordRenderSettings();
            SetDataContext();
            InitializeProperties();

            MainPage = mainPage;
        }

        private void SetDataContext()
        {
            DataContext = RenderSettings;
        }

        private void SetUpInitialWordRenderSettings()
        {
            RenderSettings = new RenderSettings()
            {
                AddFooter = true,
                AddHeader = true,
                AddLeftTable = true,
                AddSecondPage = true,
                AddTitlePage = true,
                DefaultColor = "#000000",
                DefaultTextSize = "12",
                FontFamily = "Times New Roman"
            };
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            SdwpAbstractFactory = new SdwpAbstractFactory();
            RenderersAbstractFactory = new RenderersAbstractFactory();

            WordDocumentRenderer = RenderersAbstractFactory.GetWordDocumentRender();
            CloudDocumentationService = ServiceAbstractFactory.GetCloudDocumentationService();
            CloudDocumentsService = ServiceAbstractFactory.GetCloudDocumentsService();
            LocalDocumentationService = ServiceAbstractFactory.GetLocalDocumentationService();
            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
        }

        private void InitializeProperties()
        {
            WordFilePathTextBox = wordExportFilePathTextBox;
            PageHeader = pageHeader;
        }

        #region Saving processes
        /// <summary>
        /// This method is called when the save btn is pressed. We get a local doc from main page, then check if the path is determined in the
        /// local doc and if everything is OK we save the documentation
        /// </summary>
        private async void SaveDocumentation(object sender, RoutedEventArgs e)
        {
            LocalDocumentation localDocumentation = GetMainPageLocalDocumentation();
            if (localDocumentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Нет документации для сохранения",
                    MessageBoxButton.OK);
                return;
            }

            if (localDocumentation.DocumentationPath == null)
            {
                localDocumentation.DocumentationPath = SelectFilePathForSaving();
            }

            await SaveDocumentation(localDocumentation);
        }

        /// <summary>
        /// Gets the main page documentation and documents which are related to this documentation and forms the
        /// local documentation object, even if the cloud documentation is uploaded. 
        /// </summary>
        private LocalDocumentation GetMainPageLocalDocumentation()
        {
            if (MainPage.LocalDocumentation == null)
                return null;

            Documentation documentation = MainPage.DocController.Documentation;
            List<Document> documents = MainPage.DocController.Documents;

            LocalDocumentation localDocumentation = new LocalDocumentation(documentation, documents)
            {
                DocumentationPath = MainPage.LocalDocumentation.DocumentationPath
            };

            return localDocumentation;
        }

        /// <summary>
        /// The method which actualy saves documentation
        ///</summary>
        private async Task SaveDocumentation(LocalDocumentation localDocumentation)
        {
            try
            {
                await LocalDocumentationService.CreateLocalDocumentationFile(localDocumentation);
                SDWPMessageBox.ShowSDWPMessageBox("Статус сохранения", "Документация успешно сохранена",
                    MessageBoxButton.OK);
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
        /// This methods is called when save documentation as btn is pressed.
        /// </summary>
        private async void SaveDocumentationAs(object sender, RoutedEventArgs e)
        {
            string savingFilePath = SelectFilePathForSaving();

            if (savingFilePath == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Ошибка выбора файла для сохранения",
                    MessageBoxButton.OK);
                return;
            }

            LocalDocumentation localDocumentation = GetMainPageLocalDocumentation();
            if (localDocumentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Нет документации для сохранения",
                    MessageBoxButton.OK);
                return;
            }

            localDocumentation.DocumentationPath = savingFilePath;

            await SaveDocumentation(localDocumentation);
        }

        /// <summary>
        /// Select a file where to save the documentation
        /// </summary>
        private string SelectFilePathForSaving()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "(*sdwp)|*sdwp",
                Title = "Выберете файл для сохранения",
                CreatePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;
                if (path.Substring(path.Length - 5) != ".sdwp")
                    path += ".sdwp";

                return path;
            }

            return null;
        }
        #endregion

        private async void SaveCloudDocumentation(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainPage.DocController.Documentation != null &&
                    MainPage.DocController.Documentation.StorageType == StorageType.Cloud)
                {
                    Documentation documentation = MainPage.DocController.Documentation;
                    List<Document> documents = MainPage.DocController.Documents;

                    await CloudDocumentationService.UpdateDocumentation(documentation);
                    await SynchronizeDocuments(documentation, documents);

                    SDWPMessageBox.ShowSDWPMessageBox("Успешно", "Докуемнтация успешно сохранена",
                        MessageBoxButton.OK);
                }
                else
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Нет облачной документации для сохранения",
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
        /// Synchronizes the new document list after it was changed locally with the database
        /// </summary>
        private async Task SynchronizeDocuments(Documentation documentation, List<Document> documents)
        {
            List<Document> databaseDocuments = (await CloudDocumentsService.
                GetDocumentationDocuments(documentation.ID)).ToList();

            //update the documents which were updated and add new ones
            foreach (Document document in documents)
            {
                if (databaseDocuments.FindIndex(d => d.ID == document.ID) > -1)
                {
                    await CloudDocumentsService.UpdateDocument(document);
                }
                else
                {
                    await CloudDocumentsService.InsertDocument(document);
                }
            }

            //delete the documents from the database which were deleted locally
            foreach (Document databaseDocument in databaseDocuments)
            {
                if (documents.FindIndex(d => d.ID == databaseDocument.ID) == -1)
                {
                    await CloudDocumentsService.DeleteDocument(databaseDocument);
                }
            }
        }

        #region Word export
        private void SelectWordFileForSaving(object sender, RoutedEventArgs e)
        {
            string filePath = FolderDialog.ShowDialog();

            if (filePath != null)
            {
                WordFilePathTextBox.Text = filePath;
            }
        }

        private async void ExportDocumentationToWord(object sender, RoutedEventArgs e)
        {
            try
            {
                PageHeader.SwitchOnTopLoader();

                Documentation documentation = MainPage.DocController.Documentation;
                List<Document> documents = MainPage.DocController.Documents;

                foreach (Document document in documents)
                {
                    WordDocumentRenderer.SetRenderParams(RenderSettings, document,
                        documentation);

                    await WordDocumentRenderer.RenderDocument();
                }

                SDWPMessageBox.ShowSDWPMessageBox("Успех", "Документация успешно срендерина в Word", MessageBoxButton.OK);
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
        #endregion
    }
}
