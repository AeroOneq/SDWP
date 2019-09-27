using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SDWP.Interfaces
{
    public interface IExceptionHandler
    {
        Dispatcher Dispatcher { get; set; }

        void HandleWithMessageBox(Exception ex);
    }
}
