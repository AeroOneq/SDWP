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

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.Services;

using SDWP.Interfaces;
using SDWP.Exceptions;
using SDWP.Factories;

using Microsoft.Win32;

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

        #region Properties
        private MainPage MainPage { get; }
        private ILocalDocumentationStorage LocalDocumentationStorage { get; set; }
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }

        private string FilePath { get; } = @"C:\Users\Aero\Desktop\Курсач\SDWP\SDWP\SDWP\bin\Debug\Docs\";
        #endregion

        public ExportDocumentationPage(MainPage mainPage)
        {
            InitializeComponent();
            InitializeService();

            MainPage = mainPage;
        }

        private void InitializeService()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            SdwpAbstractFactory = new SdwpAbstractFactory();

            LocalDocumentationStorage = ServiceAbstractFactory.GetLocalStorageService();
            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
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
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Нет документации для сохранения", MessageBoxButton.OK);
                return;
            }

            if (localDocumentation.DocumentationPath == null)
            {
                localDocumentation.DocumentationPath = SelectFilePathForSaving();
            }

            await SaveDocumentation(localDocumentation);
        }

        /// <summary>
        /// Gets the main page documentation and documents which are related to this documentation
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
                await LocalDocumentationStorage.CreateLocalDocumentationFile(localDocumentation, localDocumentation.DocumentationPath);
                SDWPMessageBox.ShowSDWPMessageBox("Статус сохранения", "Документация успешно сохранена", MessageBoxButton.OK);
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
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Ошибка выбора файла для сохранения", MessageBoxButton.OK);
                return;
            }

            LocalDocumentation localDocumentation = GetMainPageLocalDocumentation();
            if (localDocumentation == null)
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Нет документации для сохранения", MessageBoxButton.OK);
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
    }
}
