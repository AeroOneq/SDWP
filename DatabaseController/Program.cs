using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Database;
using AeroORMFramework;
using ApplicationLib.Models;

using FileLib;

namespace DatabaseController
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WordDB wordDB = new WordDB(new Documentation(), new List<Document>(), "");
            await wordDB.CreateWordDocument();
        }
    }
}
