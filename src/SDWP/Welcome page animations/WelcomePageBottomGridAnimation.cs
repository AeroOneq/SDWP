using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
                Duration = TimeSpan.FromMilliseconds(200),
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
                Duration = TimeSpan.FromMilliseconds(200),
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
