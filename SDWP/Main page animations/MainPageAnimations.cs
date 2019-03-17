using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SDWP
{
    public class MainPageAnimations
    {
        public static void HideLeftDocumentationGrid(Grid leftDocGrid)
        {
            leftDocGrid.Children.OfType<Grid>().ToList()[2].Visibility = Visibility.Collapsed;
            DoubleAnimation hideAnimation = new DoubleAnimation()
            {
                To = 60,
                Duration = TimeSpan.FromMilliseconds(200),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            hideAnimation.Completed += (sender, e) =>
            {
                leftDocGrid.Width = 60;
            };

            leftDocGrid.BeginAnimation(FrameworkElement.WidthProperty, hideAnimation);
        }

        public static void ShowLeftDocumentationGrid(Grid leftDocGrid)
        {
            DoubleAnimation hideAnimation = new DoubleAnimation()
            {
                To = 250,
                Duration = TimeSpan.FromMilliseconds(200),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            hideAnimation.Completed += (sender, e) =>
            {
                leftDocGrid.Width = 250;
                leftDocGrid.Children.OfType<Grid>().ToList()[2].Visibility = Visibility.Visible;
            };

            leftDocGrid.BeginAnimation(FrameworkElement.WidthProperty, hideAnimation);
        }

        public static void AnimateMargin(Grid grid, Thickness newMargin)
        {
            ThicknessAnimation marginAnimation = new ThicknessAnimation()
            {
                To = newMargin,
                Duration = TimeSpan.FromMilliseconds(200),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            marginAnimation.Completed += (sender, e) =>
            {
                grid.Margin = newMargin;
            };

            grid.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
        }
    }
}
