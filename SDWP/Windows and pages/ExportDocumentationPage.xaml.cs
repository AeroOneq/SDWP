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

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для ExportDocumentationPage.xaml
    /// </summary>
    public partial class ExportDocumentationPage : Page
    {
        private MainPage MainPage { get; }

        public ExportDocumentationPage(MainPage mainPage)
        {
            InitializeComponent();

            MainPage = mainPage;
        }


        #region Save processes
        private void SaveDocumentation(object sender, RoutedEventArgs e)
        {
        }

        private async void WriteLocalDocumentationToFile(LocalDocumentation localDocumentation, string filePath)
        {
        }

        private void SaveDocumentationAs(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
