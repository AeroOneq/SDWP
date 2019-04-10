using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using SDWP.Factories;
using SDWP.Exceptions;

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
    }
}
