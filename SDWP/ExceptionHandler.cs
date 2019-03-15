using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SDWP
{
    public static class ExceptionHandler
    {
        #region Propperties/Variables
        public static Dispatcher Dispatcher { get; set; }
        private static string MessageCaption { get; } = "SDWP Soft's message";
        #endregion
        #region Methods
        public static void HandleWithMessageBox(Exception ex)
        {
            Dispatcher.Invoke(() => SDWPMessageBox.ShowSDWPMessageBox(
                "SDWP's message", ex.Message, MessageBoxButton.OK));
        }
        #endregion
    }
}
