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


namespace SDWP
{
    public static class WelcomePageRightGridAnimations
    {
        public static List<Grid> RightGridsList { get; set; }
        #region Show the right grids
        public static void ShowTheGrid(Grid grid)
        {
            MakeAllRightGridsCollapsed();
            LowerTheGrid(grid);
        }
        public static void HideTheGrid(Grid grid)
        {
            UpperTheGrid(grid);
        }
        private static void UpperTheGrid(Grid grid)
        {
            ThicknessAnimation upperAnimation = new ThicknessAnimation
            {
                From = grid.Margin,
                To = new Thickness(0, -500, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop,
            };
            upperAnimation.Completed += (sender, eArgs) =>
            {
                grid.Visibility = Visibility.Collapsed;
            };
            grid.BeginAnimation(FrameworkElement.MarginProperty, upperAnimation);
            //this is the application description grid, we make it visible in the end of the animation
            RightGridsList[0].Visibility = Visibility.Visible;
        }
        private static void LowerTheGrid(Grid grid)
        {
            SetInitalParamsOfAnimatedGrid(grid);
            ThicknessAnimation lowerAnimation = new ThicknessAnimation
            {
                From = grid.Margin,
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            lowerAnimation.Completed += (sender, eArgs) =>
            {
                SetFinalParamsOfAnimatedGrid(grid);
            };
            grid.BeginAnimation(FrameworkElement.MarginProperty, lowerAnimation);
        }
        private static void SetFinalParamsOfAnimatedGrid(Grid grid)
        {
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Visibility = Visibility.Visible;
        }
        private static void SetInitalParamsOfAnimatedGrid(Grid grid)
        {
            grid.Margin = new Thickness(0, -500, 0, 0);
            grid.Visibility = Visibility.Visible;
        }
        private static void MakeAllRightGridsCollapsed()
        {
            foreach (Grid grid in RightGridsList)
            {
                grid.Visibility = Visibility.Collapsed;
            }
        }
        private static void MakeAllGridCollapsedInsteadOfOne(Grid grid)
        {
            foreach (Grid gridFromList in RightGridsList)
            {
                if (gridFromList != grid)
                    grid.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
