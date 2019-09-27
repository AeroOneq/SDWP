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
using System.Windows.Shapes;
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

using SDWP.Factories;
using SDWP.Interfaces;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для CreateNewDocumentationWindow.xaml
    /// </summary>
    public partial class CreateNewDocumentationWindow : Window
    {
        #region Services and factories
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }

        private IExceptionHandler ExceptionHandler { get; set; }
        private ILocalDocumentationService LocalDocumentationService { get; }
        private ICloudDocumentationService CloudDocumentationService { get; }
        #endregion

        #region Properties
        private TextBox DocumentationNameTextBox { get; set; }
        #endregion

        #region Constructors and initialize methods
        public CreateNewDocumentationWindow()
        {
            InitializeComponent();
            InitializeProperties();
            InitializeServices();
        }

        public CreateNewDocumentationWindow(ILocalDocumentationService localDocumentationService)
            : this()
        {
            LocalDocumentationService = localDocumentationService;
        }

        public CreateNewDocumentationWindow(ICloudDocumentationService cloudDocumentationService)
            : this()
        {
            CloudDocumentationService = cloudDocumentationService;
        }

        private void InitializeProperties()
        {
            DocumentationNameTextBox = documentationNameTextBox;
        }

        private void InitializeServices()
        {
            SdwpAbstractFactory = new SdwpAbstractFactory();

            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
        }
        #endregion

        #region Event handlers
        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void CreateNewDocumentation(object sender, RoutedEventArgs e)
        {
            try
            {
                string documentationName = DocumentationNameTextBox.Text;

                if (string.IsNullOrEmpty(documentationName) || string.IsNullOrWhiteSpace(documentationName))
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Введите имя документации", MessageBoxButton.OK);
                    return;
                }

                Documentation documentation = GetNewDocumentation(documentationName);
                if (CloudDocumentationService == null)
                {
                    documentation.StorageType = StorageType.Local;
                    LocalDocumentation localDocumentation = new LocalDocumentation(documentation, new List<Document>());
                    localDocumentation.DocumentationPath = System.IO.Path.Combine(LocalDocumentationService.StoragePath,
                        localDocumentation.Documentation.Name + ".sdwp");

                    await LocalDocumentationService.CreateLocalDocumentationFile(localDocumentation);
                }
                else
                {
                    documentation.StorageType = StorageType.Cloud;
                    await CloudDocumentationService.InsertDocumentation(documentation);
                }

                DialogResult = true;
                Close();
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

        private Documentation GetNewDocumentation(string name)
        {
            return new Documentation()
            {
                Access = Access.Private,
                AuthorID = UserInfo.CurrentUser.ID,
                AuthorName = UserInfo.CurrentUser.Name,
                CreationDate = DateTime.Now,
                Name = name,
                UpdatedAt = DateTime.Now,
                ManagerName = string.Empty,
                ProjectCode = string.Empty,
                ProjectName = string.Empty,
                SoftwareEngineerName = string.Empty,
                TeamLeadName = string.Empty
            };
        }
        #endregion
    }
}
