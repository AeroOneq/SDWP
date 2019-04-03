using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

namespace FileLib.Interfaces
{
    public interface IFileParser
    {
        Table[] GetAssemblyTables(string filePath, Item parentItem);
    }
}
