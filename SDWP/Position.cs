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
using AeroORMFramework;
using System.Data.SqlTypes;
using System.Net.Mail;

using SDWP.Models;

namespace SDWP
{
    /// <summary>
    /// Class where all operations connected with size and position of
    /// the elements are
    /// </summary>
    public class Position
    {
        #region Constants (sizes)
        private const double LeftMenuOptionGridWidth = 230;
        #endregion

        #region Properties 
        public SDWPMainWindow MainWindow { get; private set; }
        #endregion

        #region Singleton
        private static Position position;
        public static Position PositionObj
        {
            get
            {
                if (position == null)
                    position = new Position();
                return position;
            }
        }
        #endregion

        #region Constrcutors
        private Position() { }
        #endregion

        public void UpdateMainWindow(SDWPMainWindow mainWindow)
        {
            MainWindow = mainWindow;
            MainWindow.Width = SystemParameters.FullPrimaryScreenWidth;
            MainWindow.Height = SystemParameters.FullPrimaryScreenHeight;
        }

        #region Position methods
        /// <summary>
        /// Sets the position and size of the top and main grid 
        /// </summary>
        public void InitializeMainGrids(MainGrids mainGrids)
        {
            mainGrids.TopOptionsGrid.Width = MainWindow.Width;
            mainGrids.MainGrid.Width = MainWindow.Width;
            mainGrids.MainGrid.Height = MainWindow.Height;
        }

        public void InitializeUserAccountGrids(UserAccountGrids userAccountGrids)
        {
            userAccountGrids.UserAccGrid.Width = (double)3 / 4 * MainWindow.Width;
            userAccountGrids.UserAccGrid.Height = MainWindow.Height;
            userAccountGrids.LeftMenuGrid.Width = LeftMenuOptionGridWidth;
            userAccountGrids.LeftMenuGrid.Height = MainWindow.Height;
            userAccountGrids.LeftMenuGrid.Margin = new Thickness(0, 0, 0, 0);
            userAccountGrids.ContentFrame.Width = (double)3 / 4 * MainWindow.Width
                - LeftMenuOptionGridWidth;
            userAccountGrids.ContentFrame.Height = MainWindow.Height;
            userAccountGrids.ContentFrame.Margin = new Thickness(
                userAccountGrids.LeftMenuGrid.Width, 0, 0, 0);
        }

        public void InitializeUserAccLeftMenuGrids(UserAccLeftMenuGrids leftMenuGrids)
        {
            leftMenuGrids.HeaderBottomLineRect.Width = (double)3 / 4 * MainWindow.Width - 78;

            leftMenuGrids.OptionsGrid.Width = LeftMenuOptionGridWidth;
            for (int i = 0; i < leftMenuGrids.OptionsGrid.Children.Count; i++)
            {
                Grid optionGrid = leftMenuGrids.OptionsGrid.Children[i] as Grid;
                optionGrid.Width = leftMenuGrids.OptionsGrid.Width;
            }
        }

        #region User Grid pages resize methods
        public void InitializeUserAccGridPagesParams(Frame parentFrame)
        {
            (parentFrame.Content as Page).Width = parentFrame.Width;
        }
        #endregion
        #endregion
    }
}
