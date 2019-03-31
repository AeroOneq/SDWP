using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using ApplicationLib.Models;

using SDWP.Exceptions;

namespace SDWP.Factories
{
    interface ISdwpAbstractFactory
    {
        IDocController GetDocController(Documentation documentation, List<Document> documents);
        IExceptionHandler GetExceptionHandler(Dispatcher dispatcher);
    }
}
