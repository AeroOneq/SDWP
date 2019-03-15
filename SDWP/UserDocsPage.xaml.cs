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
namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для UserDocsPage.xaml
    /// </summary>
    public partial class UserDocsPage : Page
    {
        #region Propeties
        private ObservableCollection<DocumentationListBoxItem> documentationsCollection =
            new ObservableCollection<DocumentationListBoxItem>()
            {
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" },
                new DocumentationListBoxItem() { DocumentationTitle = "SDWPTestDocumentation" }
            };
        #endregion
        public UserDocsPage()
        {
            InitializeComponent();
            documentationsListBox.ItemsSource = documentationsCollection;
        }

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
        private void RefreshIconMouseEnter(object sender, EventArgs e)
        {
            refreshIconActive.Visibility = Visibility.Visible;
            refreshIconStatic.Visibility = Visibility.Collapsed;
        }
        private void RefreshIconMouseLeave(object sender, EventArgs e)
        {
            refreshIconActive.Visibility = Visibility.Collapsed;
            refreshIconStatic.Visibility = Visibility.Visible;
        }
        private async void RefreshUserDocs(object sender, EventArgs e)
        {
            SwitchOnTopLoader();
            await Task.Delay(1000);
            SwitchOffTheLoader();
        }
        private void UserDocsPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            headerRect.Width = this.Width;
        }
        #endregion

        #region Top loader operations
        private void SwitchOnTopLoader()
        {
            topLoaderGrid.Visibility = Visibility.Visible;
            List<Ellipse> ellipsesList = topLoaderGrid.Children.Cast<Ellipse>().
                ToList();

            ellipsesList[2].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderThirdEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
            ellipsesList[1].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderSecondEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
            ellipsesList[0].BeginAnimation(FrameworkElement.MarginProperty,
                this.Resources["topLoaderFirstEllipseAnimation"] as
                ThicknessAnimationUsingKeyFrames);
        }
        private void SwitchOffTheLoader()
        {
            List<Ellipse> ellipsesList = topLoaderGrid.Children.Cast<Ellipse>().
                 ToList();

            ellipsesList[2].BeginAnimation(FrameworkElement.MarginProperty, null);
            ellipsesList[1].BeginAnimation(FrameworkElement.MarginProperty, null);
            ellipsesList[0].BeginAnimation(FrameworkElement.MarginProperty, null);
        }
        #endregion
    }
}
