using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Database;
using AeroORMFramework;
using ApplicationLib.Models;
using ApplicationLib.Word;

using FileLib;

namespace DatabaseController
{
    class Program
    {
        private static Document Document { get; set; }
        static async Task Main()
        {
            DocumentsDB documentsDB = new DocumentsDB();
            Document = (await documentsDB.GetDocumentationDocuments(8)).ToList()[0];

            WordRenderSettings wordRenderSettings = new WordRenderSettings()
            {
                AddFooter = true,
                AddHeader = true,
                AddPageNumber = true,
                DefaultColor = "#000000",
                DefaultTextSize = "24",
                FontFamily = "Times New Roman"
            };

            WordDB wordDB = new WordDB(Document, wordRenderSettings);
            await wordDB.CreateWordDocument();
        }
    }
}
