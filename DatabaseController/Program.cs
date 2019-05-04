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
            Connector connector = new Connector(DatabaseProperties.ConnectionString);
            connector.UpdateTable<Documentation>();
            /*DocumentsDB documentsDB = new DocumentsDB();
            Document = (await documentsDB.GetDocumentationDocuments(8)).ToList()[0];

            RenderSettings wordRenderSettings = new RenderSettings()
            {\
                AddFooter = true,
                AddHeader = true,
                DefaultColor = "#000000",
                DefaultTextSize = "24",
                FontFamily = "Times New Roman"
            };

            WordRenderer wordDB = new WordRenderer();
            await wordDB.RenderDocument();*/
            Console.WriteLine("Success");
        }
    }
}
