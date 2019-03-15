using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
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
using DataStructures;
using Exceptions;

namespace SDWP
{
    public static class WelcomePageBottomGridAnimation
    {
        public static void ShowTheHintGrid(Grid grid)
        {
            SetIntialParameters(grid);
            grid.BeginAnimation(FrameworkElement.MarginProperty, CreateShowAnimation(grid));
        }
        public static void CloseTheHintGrid(Grid grid)
        {
            grid.BeginAnimation(FrameworkElement.MarginProperty, CreateHideAnimation(grid));
        }
        private static void SetIntialParameters(Grid grid)
        {
            grid.Margin = new Thickness(0, 0, 0, -100);
            grid.Visibility = Visibility.Visible;
        }
        private static ThicknessAnimation CreateHideAnimation(Grid grid)
        {
            ThicknessAnimation hideAnimation = new ThicknessAnimation
            {
                From = grid.Margin,
                To = new Thickness(0, 0, 0, -125),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop,
            };
            hideAnimation.Completed += (sender, e) =>
            {
                grid.Visibility = Visibility.Collapsed;
            };
            return hideAnimation;
        }
        private static ThicknessAnimation CreateShowAnimation(Grid grid)
        {
            ThicknessAnimation showAnimation = new ThicknessAnimation
            {
                From = new Thickness(0, 0, 0, -125),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop,
            };
            showAnimation.Completed += (sender, e) => SetFinalParameters(grid);
            return showAnimation;
        }
        private static void SetFinalParameters(Grid grid)
        {
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Visibility = Visibility.Visible;
        }
    }
}
