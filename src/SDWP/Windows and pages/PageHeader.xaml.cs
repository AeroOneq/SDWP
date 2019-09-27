using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDWP
{
    public partial class PageHeader : UserControl
    {
        #region Properties
        public Action OnRefresh { get; set; }

        public string Header { get; set; }
        public bool IsRefreshEnabled { get; set; }
        #endregion

        public PageHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(prop, new PropertyChangedEventArgs(prop));
        }
        #endregion

        #region Event handlers
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

        private void TempaltePageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            headerRect.Width = this.Width;
        }

        private void Refresh(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SwitchOnTopLoader();
                OnRefresh();
            }
            finally
            {
                SwitchOffTheLoader();
            }
        }
        #endregion

        #region Loader animations
        public void SwitchOnTopLoader()
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
        public void SwitchOffTheLoader()
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
