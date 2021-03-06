﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;
using ApplicationLib.FileParsers.Parsers;
using ApplicationLib.FileParsers.Interfaces;

using SDWP.Exceptions;
using SDWP.Interfaces;

namespace SDWP.Interfaces
{
    interface ISdwpAbstractFactory
    {
        IDocController GetDocController();
        IExceptionHandler GetExceptionHandler(Dispatcher dispatcher);
        IFileParser GetFileParser();
    }
}
