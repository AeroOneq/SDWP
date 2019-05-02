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
        private static Document Document { get; set; } = new Document()
        {
            Items = new List<Item>()
            {
                new Item()
                {
                    Name = "First item",
                    Items = new List<Item>()
                    {
                        new Item()
                        {
                            Name = "Second item"
                        },
                        new Item()
                        {
                            Name = "Third item"
                        },
                        new Item()
                        {
                            Name = "Fourth item",
                            Items = new List<Item>()
                            {
                                new Item()
                                {
                                    Name = "Fifth item",
                                    Items = new List<Item>()
                                    {
                                        new Item()
                                        {
                                            Name = "Sixth item",
                                            Paragraphs = new List<Paragraph>()
                                            {
                                                new Paragraph("Subparagraph",
                                                new Subparagraph()
                                                {
                                                    Text= "sadksjadlas daksldsakldsa kdsakdlsa dklasdj askdasj dklasdklsadskadkasdklasdklasdjklsa kasld lkdj sakld jaskld jaskdlj saklas jdkasld jaskdjasklas "
                                                }),
                                                new Paragraph("Subparagraph",
                                                new Subparagraph()
                                                {
                                                    Text= "sadksjadlas daksldsakldsa kdsakdlsa dklasdj askdasj dklasdklsadskadkasdklasdklasdjklsa kasld lkdj sakld jaskld jaskdlj saklas jdkasld jaskdjasklas "
                                                })
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new Item()
                        {
                            Name = "Seventh item",
                            Items = new List<Item>()
                            {
                                new Item()
                                {
                                    Name = "Eighth item"
                                }
                            }
                        },
                        new Item()
                        {
                            Name = "Nineth item"
                        },
                        new Item()
                        {
                            Name = "Tenth item"
                        }
                    }
                }
            }
        };
        static async Task Main(string[] args)
        {
            DocumentsDB documentsDB = new DocumentsDB();
            Document = (await documentsDB.GetDocumentationDocuments(8)).ToList()[0];

            WordDB wordDB = new WordDB(Document);
            await wordDB.CreateWordDocument();
        }
    }
}
