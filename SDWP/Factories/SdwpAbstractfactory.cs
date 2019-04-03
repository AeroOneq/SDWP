using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using ApplicationLib.Models;

using FileLib.Interfaces;
using FileLib.FileParsers;

using SDWP.Exceptions;

namespace SDWP.Factories
{
    class SdwpAbstractFactory : ISdwpAbstractFactory
    {
        public IDocController GetDocController(Documentation documentation, List<Document> documents)
        {
            return new DocumentationController(documentation, documents);
        }

        public IExceptionHandler GetExceptionHandler(Dispatcher dispatcher)
        {
            return new ExceptionHandler(dispatcher);
        }

        public IFileParser GetFileParser()
        {
            return new CSFileParser();
        }
    }
}
