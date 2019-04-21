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

using SDWP.Interfaces;

namespace SDWP.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        #region Propperties/Variables
        public Dispatcher Dispatcher { get; set; }
        private string MessageCaption { get; } = "SDWP Soft's message";
        #endregion

        public ExceptionHandler(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        #region Methods
        public void HandleWithMessageBox(Exception ex)
        {
            Dispatcher.Invoke(() => SDWPMessageBox.ShowSDWPMessageBox(
                "SDWP's message", ex.Message, MessageBoxButton.OK));
        }
        #endregion
    }
}
