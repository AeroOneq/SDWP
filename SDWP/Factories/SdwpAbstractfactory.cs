using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using ApplicationLib.Models;
using ApplicationLib.FileParsers.Interfaces;
using ApplicationLib.FileParsers.Parsers;

using SDWP.Exceptions;
using SDWP.Interfaces;

namespace SDWP.Factories
{
    class SdwpAbstractFactory : ISdwpAbstractFactory
    {
        public IDocController GetDocController()
        {
            return new DocumentationController();
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
