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

            LocalDocumentationStorage = ServiceAbstractFactory.GetLocalStorageService();
        }


        #region Save processes
        private void SaveDocumentation(object sender, RoutedEventArgs e)
        {
            LocalDocumentation localDocumentation = MainPage.LocalDocumentation;
            LocalDocumentationStorage.CreateLocalDocumentationFile(localDocumentation, localDocumentation.DocumentationPath);
            SDWPMessageBox.ShowSDWPMessageBox("Статус сохранения", "Документация успешно сохранена", MessageBoxButton.OK);
        }

        private void SaveDocumentationAs(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
