using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using SDWP.Factories;
using SDWP.Exceptions;
using SDWP.Interfaces;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SDWP
{
    public partial class App : Application
    {
        private static ISdwpAbstractFactory AbstractFactory { get; set; }
        private static IExceptionHandler ExceptionHandler { get; set; }

        App()
        {
            InitializeComponent();

            AbstractFactory = new SdwpAbstractFactory();
            ExceptionHandler = AbstractFactory.GetExceptionHandler(Dispatcher);
        }

        [STAThread]
        static void Main()
        {
            string guid = Marshal.GetTypeLibGuidForAssembly(Assembly.GetExecutingAssembly()).ToString();
            using (var mutex = new Mutex(true, guid))
            {
                try
                {
                    if (!mutex.WaitOne(TimeSpan.FromMilliseconds(5), false))
                    {
                        SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Приложение уже запущено",
                            MessageBoxButton.OK);

                        return;
                    }

                    App app = new App();
                    MainWindow mainWindow = new MainWindow();

                    app.Run(mainWindow);
                }
                catch (Exception ex)
                {
                    SDWPMessageBox.ShowSDWPMessageBox("Фатальная ошибка", "Произошла фатальная ошибка," +
                        "приложение будет закрыто, попробуйте перезапустить или перустановить его. " +
                        "Сейчас будет показано сообщение об ошибке.",
                        MessageBoxButton.OK);

                    ExceptionHandler.HandleWithMessageBox(ex);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
