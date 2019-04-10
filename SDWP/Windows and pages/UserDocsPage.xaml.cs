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
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;
using ApplicationLib.Services;

using SDWP.Models;
using SDWP.Factories;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для UserDocsPage.xaml
    /// </summary>
    public partial class UserDocsPage : Page
    {
        #region Propeties
        private PageHeader PageHeader { get; }
        private string DefaultFilePath { get; set; } = @"C:\Users\Aero\Desktop\Курсач\SDWP\SDWP\SDWP\bin\Debug\Docs\";

        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ILocalDocumentationStorage LocalDocumentationService { get; set; }

        private List<LocalDocumentation> LocalDocumentations { get; set; }
        #endregion

        public UserDocsPage()
        {
            InitializeComponent();
            InitializeServices();

            PageHeader = pageHeader;
            UploadDocumentationsFromLocalSotrage();
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            LocalDocumentationService = ServiceAbstractFactory.GetLocalStorageService(DefaultFilePath);
        }

        #region Upload documentation methods
        private async Task UploadDocumentationsFromLocalSotrage()
        {
            LocalDocumentations = await LocalDocumentationService.GetLocalDocumentations();

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
    }
}
