using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using AeroORMFramework;
using System.Data.SqlTypes;
using System.Net.Mail;

using ApplicationLib.Models;

using Newtonsoft.Json;

using SDWP.Models;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для SDWPMainWindow.xaml
    /// </summary>
    public partial class SDWPMainWindow : Window
    {
        #region Properties
        private MainGrids MainGrids { get; set; }
        private UserAccountGrids UserAccountGrids { get; set; }
        private UserAccLeftMenuGrids UserAccLeftMenuGrids { get; set; }
        private MainPage MainPage { get; }

        private UserDocsPage UserDocsPage { get; set; }
        private ExportDocumentationPage ExportDocumentationPage { get; set; }
        private UserProfilePage UserProfilePage { get; set; }
        private DocumentTemplatesPage DocumentTemplatesPage { get; set; }
        #endregion

        #region Constructors
        public SDWPMainWindow(UserInfo user)
        {
            InitializeComponent();
            InitializePositionObjects();
            InitializeInitialPosition();

            Position.PositionObj.UpdateMainWindow(this);

            mainPageFrame.Content = MainPage = new MainPage();
        }
        #endregion

        #region Initializing methods
        /// <summary>
        /// Initialize the initial position of this window (top and left margin)
        /// </summary>
        private void InitializeInitialPosition()
        {
            Top = 0;
            Left = 0;
        }

        /// <summary>
        /// Initializes the objects which are then used in Position class to resize elements when the
        /// window is being resized
        /// </summary>
        private void InitializePositionObjects()
        {
            MainGrids = new MainGrids
            {
                TopOptionsGrid = topOptionsGrid,
                MainGrid = documentationGrid,
                UserAccountGrid = userAccMainGrid
            };
            UserAccountGrids = new UserAccountGrids
            {
                UserAccGrid = userAccMainGrid,
                LeftMenuGrid = userGridLeftMenuGrid,
                ContentFrame = userGridFrame,
            };
            UserAccLeftMenuGrids = new UserAccLeftMenuGrids
            {
                HeaderBottomLineRect = headerBottomLineRect,
                OptionsGrid = leftMenuOptionsGridsGrid,
            };
        }
        #endregion

        #region Size changed events
        /// <summary>
        /// Resizes the elements when user changes the size of the window 
        /// </summary>
        private void SDWPMainWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                Position.PositionObj.UpdateMainWindow(this);
            if (userAccMainGrid.Visibility == Visibility.Visible)
            {
                Position.PositionObj.InitializeUserAccountGrids(UserAccountGrids);
                Position.PositionObj.InitializeUserAccLeftMenuGrids(UserAccLeftMenuGrids);
                Position.PositionObj.InitializeUserAccGridPagesParams(userGridFrame);
            }
            else
                Position.PositionObj.InitializeMainGrids(MainGrids);
        }
        #endregion

        #region Top Grids operations
        /// <summary>
        /// When the mouse in the suitable area of this window this method shows the top options grid,
        /// when the mouse leaves the area where top options grid is this method hides the top option grid
        /// </summary>
        private void SDWPMainWindowMouseMove(object sender, MouseEventArgs e)
        {
            Point cursorCoordinates = e.GetPosition(this);

            if (cursorCoordinates.Y < 5 && cursorCoordinates.X < 500 && topOptionsGrid.Height == 0)
            {
                ShowTopGrids();
            }
            else if (cursorCoordinates.Y > 100 && topOptionsGrid.Height == 100)
            {
                HideTopGrids();
            }
        }

        /// <summary>
        /// Changes the colors and font weight of the elements in one of 
        /// the options element
        /// </summary>
        private void OptionGridMouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            List<TextBlock> gridTextBlocksList = grid.Children.OfType<TextBlock>().ToList();
            gridTextBlocksList[0].FontWeight = FontWeights.Bold;
            grid.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250));

            List<Image> iconsList = grid.Children.OfType<Image>().ToList();
            iconsList[0].Visibility = Visibility.Collapsed;
            iconsList[1].Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Changes everything which was changed in the 
        /// previous method to its original values
        /// </summary>
        private void OptionGridMouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            //make the text usual
            List<TextBlock> gridTextBlocksList = grid.Children.OfType<TextBlock>().ToList();
            gridTextBlocksList[0].FontWeight = FontWeights.Normal;
            //change the background back
            grid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //set the static image visible
            List<Image> iconsList = grid.Children.OfType<Image>().ToList();
            iconsList[0].Visibility = Visibility.Visible;
            iconsList[1].Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes the background of the hide options grid
        /// </summary>
        private void HideOptionsGridMouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
        }

        /// <summary>
        /// Chenges the background of the hide options grid to its original value
        /// </summary>
        private void HideOptionsGridMouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Background = new SolidColorBrush(Color.FromRgb(211, 211, 211));
        }

        /// <summary>
        /// Hides the top option grid when the mouse leaves the top option grid
        /// </summary>
        private void HideTopGrids()
        {
            topOptionsGrid.BeginAnimation(HeightProperty,
                CreateUpTopGridAnimation());
            mainPageFrame.BeginAnimation(MarginProperty,
                CreateDocumentFrameUpAnimation());
        }

        /// <summary>
        /// Shows top option grids when the mouse is in a suitable position (under the documents panel)
        /// </summary>
        private void ShowTopGrids()
        {
            topOptionsGrid.BeginAnimation(HeightProperty,
                CreateDownTopGridAnimation());
            mainPageFrame.BeginAnimation(MarginProperty,
                CreateDocumentFrameDownAnimation());
        }

        /// <summary>
        /// Creates ans returns the animation which is used to hide the top options grid
        /// This animation animates the height of the grid
        /// </summary>
        private DoubleAnimation CreateUpTopGridAnimation()
        {
            DoubleAnimation optionsGridHeightAnimation = new DoubleAnimation
            {
                From = 100,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };

            optionsGridHeightAnimation.Completed += (send, events) =>
            {
                topOptionsGrid.Height = 0;
            };

            return optionsGridHeightAnimation;
        }

        /// <summary>
        /// Creates and returns an animation which is used to show the top options grid
        /// This animation animates the height of the grid
        /// </summary>
        private DoubleAnimation CreateDownTopGridAnimation()
        {
            DoubleAnimation optionsGridHeightAnimation = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            optionsGridHeightAnimation.Completed += (send, events) =>
            {
                topOptionsGrid.Height = 100;
            };
            return optionsGridHeightAnimation;
        }

        /// <summary>
        /// Creates and returns an animation which is used to lower the document frame when the
        /// top option grid is being shown
        /// </summary>
        private ThicknessAnimation CreateDocumentFrameDownAnimation()
        {
            ThicknessAnimation lowerTheDocumentFrameAnimation = new ThicknessAnimation
            {
                To = new Thickness(0, 100, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            lowerTheDocumentFrameAnimation.Completed += (send, events) =>
            {
                mainPageFrame.Margin = new Thickness(0, 100, 0, 0);
            };
            return lowerTheDocumentFrameAnimation;
        }

        /// <summary>
        /// Creates and returns an animation which is used to up rise the document frame when
        /// the top options grid is being hidden
        /// </summary>
        private ThicknessAnimation CreateDocumentFrameUpAnimation()
        {
            ThicknessAnimation upperTheDocumentFrameAnimation = new ThicknessAnimation
            {
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromMilliseconds(300),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            upperTheDocumentFrameAnimation.Completed += (send, events) =>
            {
                mainPageFrame.Margin = new Thickness(0, 0, 0, 0);
            };
            return upperTheDocumentFrameAnimation;
        }
        #endregion

        #region User main grid operations
        /// <summary>
        /// After a click on an option "Account" in a top options grid shows the User main grid
        /// where all account functionality is
        /// </summary>
        private void ShowTheUserMainGrid(object sender, MouseButtonEventArgs e)
        {
            userAccMainGrid.Width = 0;

            Position.PositionObj.InitializeUserAccountGrids(UserAccountGrids);
            Position.PositionObj.InitializeUserAccLeftMenuGrids(UserAccLeftMenuGrids);

            userAccMainGrid.Visibility = Visibility.Visible;
            userAccMainGrid.BeginAnimation(WidthProperty,
                CreateShowMainUserGridAnimation());
        }

        /// <summary>
        /// Clears the history of a frame, where all pages which in User main grid are displayed
        /// </summary>
        private void ClearFrameHistory(Frame frame)
        {
            frame.Navigate(new Page());

            if (!frame.CanGoBack && !frame.CanGoForward)
                return;
            var entry = frame.RemoveBackEntry();
            while (frame.CanGoBack)
                entry = frame.RemoveBackEntry();
            frame.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
        }

        /// <summary>
        /// Creates and returns an animation which is used to show user main grid
        /// </summary>
        private DoubleAnimation CreateShowMainUserGridAnimation()
        {
            DoubleAnimation showTheGridAnimation = new DoubleAnimation
            {
                From = 0,
                To = 3 * this.Width / 4,
                Duration = TimeSpan.FromMilliseconds(200),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            showTheGridAnimation.Completed += (sender, e) =>
            {
                userAccMainGrid.Width = 3 * this.Width / 4;
            };
            return showTheGridAnimation;
        }

        /// <summary>
        /// Creates and returns an animation which is used to show user main grid
        /// </summary>
        private DoubleAnimation CreateHideMainGridAnimation()
        {
            DoubleAnimation hideTheGridAnimation = new DoubleAnimation
            {
                From = 3 * this.Width / 4,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                DecelerationRatio = 1,
                SpeedRatio = 0.5,
                FillBehavior = FillBehavior.Stop
            };
            hideTheGridAnimation.Completed += (sender, e) =>
            {
                userAccMainGrid.Width = 0;
                userAccMainGrid.Visibility = Visibility.Collapsed;
            };
            return hideTheGridAnimation;
        }

        private void CloseAccGridActiveIconMouseLeave(object sender, MouseEventArgs e)
        {
            closeAccGridStaticIcon.Visibility = Visibility.Visible;
            closeAccGridActiveIcon.Visibility = Visibility.Collapsed;
        }

        private void HideTheUserAccGrid(object sender, EventArgs e)
        {
            userAccMainGrid.BeginAnimation(WidthProperty,
                CreateHideMainGridAnimation());
        }

        private void CloseAccGridStaticIconMouseEnter(object sender, EventArgs e)
        {
            closeAccGridStaticIcon.Visibility = Visibility.Collapsed;
            closeAccGridActiveIcon.Visibility = Visibility.Visible;
        }
        #endregion

        #region User grid event handlers
        private void LeftMenuOptionGridMouseEnter(object sender, EventArgs e)
        {
            Grid optionGrid = sender as Grid;
            optionGrid.Background = new SolidColorBrush(Color.FromRgb(219, 219, 219));
            TextBlock optionGridText = optionGrid.Children[0] as TextBlock;
            optionGridText.FontWeight = FontWeights.Bold;
        }

        private void LeftMenuOptionGridMouseLeave(object sender, EventArgs e)
        {
            Grid optionGrid = sender as Grid;
            optionGrid.Background = new SolidColorBrush(Colors.LightGray);
            TextBlock optionGridText = optionGrid.Children[0] as TextBlock;
            optionGridText.FontWeight = FontWeights.Normal;
        }

        private void LeftMenuOptionGridMouseDown(object sender, EventArgs e)
        {
            Grid clickedOptionGrid = sender as Grid;
            SwitchTheMainGrid(clickedOptionGrid.Uid);
        }

        /// <summary>
        /// Uploads the page which realtes to the clicked header in user main grid to 
        /// userGridFrame
        /// </summary>
        private void SwitchTheMainGrid(string clickedGridUid)
        {
            ClearFrameHistory(userGridFrame);
            switch (clickedGridUid)
            {
                case "0":
                    if (UserProfilePage == null)
                    {
                        UserProfilePage = new UserProfilePage(UserInfo.CurrentUser)
                        {
                            Width = userGridFrame.Width,
                            CloseAccGrid = new Action(() => HideTheUserAccGrid(null, null))
                        };
                    }
                    userGridFrame.Navigate(UserProfilePage);
                    break;

                case "1":
                    if (UserDocsPage == null)
                    {
                        UserDocsPage = new UserDocsPage(MainPage)
                        {
                            Width = userGridFrame.Width,
                            CloseAccGrid = new Action(() => HideTheUserAccGrid(null, null))
                        };
                    }
                    userGridFrame.Navigate(UserDocsPage);
                    break;

                case "2":
                    if (DocumentTemplatesPage == null)
                    {
                        DocumentTemplatesPage = new DocumentTemplatesPage(UserInfo.CurrentUser)
                        {
                            Width = userGridFrame.Width,
                            Height = userGridFrame.Height,
                            CloseAccGrid = new Action(() => HideTheUserAccGrid(null, null))
                        };
                    }
                    userGridFrame.Navigate(DocumentTemplatesPage);
                    break;

                case "3":
                    if (ExportDocumentationPage == null)
                    {
                        ExportDocumentationPage = new ExportDocumentationPage(MainPage)
                        {
                            Width = userGridFrame.Width,
                            CloseAccGrid = new Action(() => HideTheUserAccGrid(null, null))
                        };
                    }
                    userGridFrame.Navigate(ExportDocumentationPage);
                    break;

                case "4":
                    QuitTheProgram();
                    break;
            }
        }
        #endregion

        #region Top option grid options methods
        /// <summary>
        /// When the save option is pressed this method opens Export documentation page 
        /// in a user main grid
        /// </summary>
        private void SaveDocumentation(object sender, EventArgs e)
        {
            ShowTheUserMainGrid(null, null);
            //3 = export documentation page
            SwitchTheMainGrid("3");
        }

        /// <summary>
        /// Opens the user doc page in the user main grid when the "delete documentation" option is pressed
        /// </summary>
        private void OpenUserDocsPage(object sender, RoutedEventArgs e)
        {
            ShowTheUserMainGrid(null, null);
            //1 = user's documentations page
            SwitchTheMainGrid("1");
        }
        #endregion


        private void QuitTheProgram()
        {
            if (SDWPMessageBox.ShowSDWPMessageBox("Подтверждение", "Вы уверены, что хотите выйти?", MessageBoxButton.OKCancel)
                == MessageBoxResult.OK)
            {
                Close();
                (new MainWindow()).Show();
            }
        }
    }
}
