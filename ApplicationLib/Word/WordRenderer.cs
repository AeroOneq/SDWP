using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace ApplicationLib.Word
{
    public class WordRenderer : IWordRenderer
    {
        private Models.Document Document { get; set; }
        private MainDocumentPart MainPart { get; set; }
        private Body Body { get; set; }
        private RenderSettings RenderSettings { get; set; }
        private Documentation Documentation { get; set; }

        #region Render properties
        private double TabValue { get; } = 500;
        #endregion

        #region Word render constants
        string[][] TitlePageTable { get; } = new string[2][]
        {
            new string[5]{"Инв. № подл", "Подп и дата", "Взаим инв №", "Инв № дубл", "Подп и дата"}.Reverse().ToArray(),
            new string[5]{ "RU.17701729.04.03-01 ТЗ", "", "", "", "" }.Reverse().ToArray()
        };
        string[][] FooterTable { get; } = new string[4][]
        {
            new string[5]{ "", "", "", "", "" },
            new string[5]{ "Изм", "Лист.", "№ Документа", "Подпись", "Дата"},
            new string[5]{ "RU.17701729.04.03-01 ТЗ", "", "", "", "" },
            new string[5]{ "Инв. № подл.", "Подп. и дата", "Взам. Инв №", "Инв. № дубл", "Подп и дата"}
        };
        #endregion


        public void SetRenderParams(RenderSettings renderSettings, Models.Document document,
            Documentation documentation)
        {
            RenderSettings = new RenderSettings(renderSettings);
            Document = document;
            Documentation = documentation;

            RenderSettings.DefaultTextSize = (int.Parse(RenderSettings.DefaultTextSize) * 2).ToString();
            TitlePageTable[1][4] = Documentation.ProjectCode;
            FooterTable[2][0] = Documentation.ProjectCode;
        }

        #region IDocumentRenderer
        public async Task RenderDocument()
        {
            await Task.Run(() =>
            {
                string filePath = Path.Combine(RenderSettings.FolderPath, Document.Name + ".docx");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                CreateDocument(filePath);
                AppendFooterAndHeader(filePath);
            });
        }
        #endregion

        private void CreateDocument(string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(
                filePath, WordprocessingDocumentType.Document, true))
            {
                MainPart = wordDocument.AddMainDocumentPart();
                MainPart.Document = new WordDocument();
                Body = new Body();

                if (RenderSettings.AddTitlePage)
                    CreateTitlePage();

                if (RenderSettings.AddSecondPage)
                    CreateSecondApprovalPage();

                CreateTableOfContents();
                RenderDocumentBody();

                Body.Append(GetSectionPtr());
                MainPart.Document.Append(Body);
            }
        }
        private void AppendFooterAndHeader(string filePath)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, true))
            {
                MainPart = wordDocument.MainDocumentPart;

                MainPart.DeleteParts(MainPart.HeaderParts);
                MainPart.DeleteParts(MainPart.FooterParts);

                HeaderPart headerPart = MainPart.AddNewPart<HeaderPart>();
                FooterPart footerPart = MainPart.AddNewPart<FooterPart>();

                string headerID = MainPart.GetIdOfPart(headerPart);
                string footerID = MainPart.GetIdOfPart(footerPart);

                if (RenderSettings.AddFooter)
                    CreateFooterContent(footerPart);

                if (RenderSettings.AddHeader)
                    CreateHeaderContent(headerPart);

                var sectionPropsList = MainPart.Document.Body.Descendants<SectionProperties>().ToList();
                var sectionProps = sectionPropsList[1];

                sectionProps.Append(new FooterReference() { Type = HeaderFooterValues.Default, Id = footerID });
                sectionProps.Append(new HeaderReference() { Type = HeaderFooterValues.Default, Id = headerID });
            }
        }

        #region Footer and Header
        private void CreateFooterContent(FooterPart footerPart)
        {
            Footer footer = new Footer() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };

            footer.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            footer.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            footer.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            footer.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            footer.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            footer.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            footer.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            footer.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            footer.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            footer.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            footer.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            footer.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            footer.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            footer.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            footer.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            WordTable wordTable = new WordTable();
            TableProperties tableProperties = new TableProperties(new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                }), new Justification() { Val = JustificationValues.Center });

            wordTable.Append(tableProperties);

            for (int i = 0; i < FooterTable.Length; i++)
            {
                TableRow tableRow = new TableRow();
                for (int j = 0; j < FooterTable[i].Length; j++)
                {
                    TableCell tableCell = new TableCell();

                    WordParagraph paragraph = new WordParagraph();
                    ParagraphProperties pproperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines()
                        {
                            After = "0",
                            Before = "0",
                            Line = "200"
                        });

                    paragraph.Append(pproperties);

                    Run run = new Run();
                    RunProperties runProperties = new RunProperties(new RunFonts()
                    {
                        Ascii = RenderSettings.FontFamily,
                        HighAnsi = RenderSettings.FontFamily
                    })
                    {
                        Color = new Color() { Val = RenderSettings.DefaultColor },
                        FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize }
                    };
                    Text text = new Text(FooterTable[i][j]);

                    run.Append(runProperties);
                    run.Append(text);

                    paragraph.Append(run);

                    tableCell.Append(paragraph);
                    tableRow.Append(tableCell);
                }

                wordTable.Append(tableRow);
            }

            footer.Append(wordTable);

            footerPart.Footer = footer;
        }
        private void CreateHeaderContent(HeaderPart headerPart)
        {
            Header header = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            header.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            header.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            header.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            header.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            header.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            header.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            header.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            header.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            header.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            header.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            header.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            header.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            header.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            header.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            header.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            header.Append(GetHeaderContent());
            headerPart.Header = header;
        }
        private SdtBlock GetHeaderContent()
        {
            return new SdtBlock(
                            new SdtProperties(
                                new SdtId() { Val = 317275692 },
                                new SdtContentDocPartObject(
                                    new DocPartGallery() { Val = "Page Numbers (Top of Page)" },
                                    new DocPartUnique())),
                            new SdtContentBlock(
                                new WordParagraph(
                                    new ParagraphProperties(
                                        new ParagraphStyleId() { Val = "Header" },
                                        new Justification() { Val = JustificationValues.Center }),
                                    new SimpleField(
                                        new Run(
                                            new RunProperties(
                                                new NoProof(),
                                                new SpacingBetweenLines()
                                                {
                                                    After = "100",
                                                    Before = "100",
                                                    Line = "200"
                                                }),
                                            new Text("1"))
                                        { RsidRunAddition = "001F06F5" })
                                    { Instruction = " PAGE   \\* MERGEFORMAT " })
                                { RsidParagraphAddition = "00F1559F", RsidRunAdditionDefault = "00BB29E4" },
                                new WordParagraph(
                                    new ParagraphProperties(
                                        new Caps(),
                                        new Color() { Val = RenderSettings.DefaultColor },
                                        new FontSize() { Val = RenderSettings.DefaultTextSize },
                                        new Justification() { Val = JustificationValues.Center },
                                        new SpacingBetweenLines()
                                        {
                                            After = "100",
                                            Before = "0",
                                            Line = "200"
                                        }),
                                    new Run(
                                        new RunProperties(new RunFonts()
                                        {
                                            Ascii = RenderSettings.FontFamily,
                                            HighAnsi = RenderSettings.FontFamily
                                        }),
                                        new Text("RU.17701729.04.03-01 ТЗ")))));
        }
        #endregion

        #region Main start methods
        private void CreateTitlePage()
        {
            if (RenderSettings.AddTitlePage)
                AddTitlePageTable();

            AddOrganizationHeader();

            AddEmptyParagrahs(1);

            AddApprovalTable();

            AddEmptyParagrahs(2);

            AddDocumentTitle();
            AddDocumentName();
            AddTitlePageTitle();
            AddDocumentNumber();

            AddEmptyParagrahs(3);

            AddSoftwateEngineerSignatureTable();

            AddEmptyParagrahs(5);

            AddTownAndYearParagraph();
            Body.Append(GetLastParagraphOfThePage());
        }
        private void CreateSecondApprovalPage()
        {
            AddTitlePageTable();
            AddApprovedParagraph();

            AddEmptyParagrahs(5);

            AddDocumentTitle();
            AddDocumentName();
            AddDocumentNumber();
            AddListCountParagraph();

            AddEmptyParagrahs(1);
            Body.Append(GetLastParagraphOfThePage());
            Body.Append(GetSectionPtr());
        }
        private void CreateTableOfContents()
        {
            SdtBlock tableOfContents = new SdtBlock();
            RunProperties tocRpr = new RunProperties(new RunFonts() { HighAnsi = RenderSettings.FontFamily },
                new Color() { Val = "auto" }, new FontSize() { Val = RenderSettings.DefaultTextSize });
            SdtContentDocPartObject sdtContentDocPartObject = new SdtContentDocPartObject(
                new DocPartGallery() { Val = "Table of Contents" }, new DocPartUnique());

            SdtProperties sdtProperties = new SdtProperties(tocRpr, sdtContentDocPartObject);

            tableOfContents.Append(sdtProperties);
            tableOfContents.Append(new SdtEndCharProperties());

            SdtContentBlock sdtContentBlock = new SdtContentBlock();

            WordParagraph p = new WordParagraph();
            ParagraphProperties ppr = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };

            p.Append(ppr);

            RunProperties rpr = new RunProperties(new RunFonts() { Ascii = RenderSettings.FontFamily, HighAnsi = RenderSettings.FontFamily })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize }
            };

            Run run = new Run();
            run.Append(rpr);

            Text text = new Text("Содержание");
            run.Append(text);

            p.Append(run);

            sdtContentBlock.Append(p);

            string index = "0";
            foreach (Item item in Document.Items)
            {
                index = (int.Parse(index) + 1).ToString();
                UploadItemsToTableOfContents(item, sdtContentBlock, 0, index);
            }

            tableOfContents.Append(sdtContentBlock);

            Body.Append(tableOfContents);
            Body.Append(GetEmptyParagraph());
            Body.Append(GetLastParagraphOfThePage());
        }
        private void UploadItemsToTableOfContents(Item item, SdtContentBlock sdtContentBlock,
            int depth, string index)
        {
            AddTableOfContentsElement(sdtContentBlock, depth, index + " " + item.Name);

            string dopIndex = "0";
            if (item.Items != null)
                foreach (Item i in item.Items)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    UploadItemsToTableOfContents(i, sdtContentBlock, depth + 1, index + "." + dopIndex);
                }
        }
        private void RenderDocumentBody()
        {
            string index = "0";
            foreach (Item item in Document.Items)
            {
                index = (int.Parse(index) + 1).ToString();
                RenderItem(item, 0, index);

                Body.Append(GetLastParagraphOfThePage());
            }
        }
        private void RenderItem(Item item, int depth, string index)
        {
            if (depth == 0)
                Body.Append(GetSectionHeadParagraph(index + " " + item.Name));
            else
                Body.Append(GetParagraphHeader(index + " " + item.Name, depth));

            if (item.Items != null)
            {
                string dopIndex = "0";
                foreach (Item i in item.Items)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    RenderItem(i, depth + 1, index + "." + dopIndex);
                }
            }

            if (item.Paragraphs != null)
            {
                foreach (Models.Paragraph paragraph in item.Paragraphs)
                {
                    switch (paragraph.Type)
                    {
                        case "Table":
                            InsertTable(paragraph.ParagraphElement as Models.Table, depth);
                            break;
                        case "Subparagraph":
                            InsertSubparagraph(paragraph.ParagraphElement as Subparagraph, depth);
                            break;
                        case "NumberedList":
                            InsertNumberedList(paragraph.ParagraphElement as NumberedList, depth);
                            break;
                        case "ParagraphImage":
                            InsertParagraphImage(paragraph.ParagraphElement as ParagraphImage, depth);
                            break;
                    }
                }
            }
        }
        #endregion

        #region Document render methods
        private WordParagraph GetSectionHeadParagraph(string name)
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts() { HighAnsi = RenderSettings.FontFamily, Ascii = RenderSettings.FontFamily })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize },
                Caps = new Caps(),
                Bold = new Bold()
            };

            run.Append(runProperties);

            Text text = new Text(name);

            run.Append(text);
            p.Append(run);

            return p;
        }
        private WordParagraph GetParagraphHeader(string name, int depth)
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new Justification() { Val = JustificationValues.Left },
                new Indentation() { Left = (TabValue * depth).ToString() });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            { HighAnsi = RenderSettings.FontFamily, Ascii = RenderSettings.FontFamily })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize },
                Bold = new Bold()
            };

            Text text = new Text(name);

            run.Append(runProperties);
            run.Append(text);
            p.Append(run);

            return p;
        }
        #endregion

        #region Common methods
        private WordParagraph GetCenterParagraph(string text)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" },

            };
            run.PrependChild(runProperties);

            var wordText = new Text(text);

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
        private WordParagraph GetRightParagraph(string text)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Right },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" },

            };
            run.PrependChild(runProperties);

            var wordText = new Text(text);

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
        private void AddEmptyParagrahs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Body.Append(GetEmptyParagraph());
            }
        }
        private WordParagraph GetEmptyParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification()
                {
                    Val = JustificationValues.Center
                }
            };
            paragraph.Append(pp);
            var run = new Run();
            var runProperties = new RunProperties();
            run.PrependChild(runProperties);
            paragraph.Append(run);

            return paragraph;
        }
        private WordParagraph GetLastParagraphOfThePage()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };
            paragraph.Append(pp);

            var run = new Run(new LastRenderedPageBreak(), new Break() { Type = BreakValues.Page });
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text();

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
        private WordParagraph GetSectionPtr()
        {
            return new WordParagraph(
                new ParagraphProperties(
                    new SectionProperties()));
        }
        private Run GetTabRun()
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts() { HighAnsi = RenderSettings.FontFamily, Ascii = RenderSettings.FontFamily });
            run.Append(runProperties);
            run.Append(new TabChar());

            return run;
        }
        #endregion

        #region Title page methods
        private void AddSoftwateEngineerSignatureTable()
        {
            Body.Append(GetRightParagraph("                     Исполнитель"));
            Body.Append(GetRightParagraph("        <Введите свою должность>"));
            Body.Append(GetRightParagraph("            /<Введите своё имя>/"));
            AddEmptyParagrahs(1);
            Body.Append(GetRightParagraph("    \"___\"_________________2019"));
        }
        private void AddApprovalTable()
        {
            WordTable table = new WordTable();

            TableProperties tableProperties = new TableProperties()
            {
                TableJustification = new TableJustification() { Val = TableRowAlignmentValues.Center },
            };

            table.AppendChild(tableProperties);

            TableRow tableRow = new TableRow();
            TableCell firstCell = new TableCell();

            firstCell.Append(GetCenterParagraph("              СОГЛАСОВАННО             "));
            firstCell.Append(GetCenterParagraph("          Руководитель проекта         "));
            firstCell.Append(GetCenterParagraph("      Должность должность должность    "));
            firstCell.Append(GetCenterParagraph("      Должность должность должность    "));
            firstCell.Append(GetCenterParagraph("                                       "));
            firstCell.Append(GetRightParagraph($"                     /{Documentation.TeamLeadName}/"));
            firstCell.Append(GetRightParagraph("              \"___\"__________________"));

            tableRow.Append(firstCell);

            TableCell middleCell = new TableCell(new TableCellProperties()
            {
                TableCellWidth = new TableCellWidth() { Width = "1000" }
            });
            middleCell.Append(GetEmptyParagraph());

            tableRow.Append(middleCell);

            TableCell secondCell = new TableCell();

            secondCell.Append(GetCenterParagraph("               УТВЕРЖДАЮ               "));
            secondCell.Append(GetCenterParagraph("   Самый главный руководитель проекта  "));
            secondCell.Append(GetCenterParagraph("      Должность должность должность    "));
            secondCell.Append(GetCenterParagraph("      Должность должность должность    "));
            secondCell.Append(GetCenterParagraph("                                       "));
            secondCell.Append(GetRightParagraph($"                     /{Documentation.ManagerName}/"));
            secondCell.Append(GetRightParagraph("              \"___\"__________________"));

            tableRow.Append(secondCell);

            table.Append(tableRow);
            Body.Append(table);
        }
        private void AddTitlePageTable()
        {
            WordTable table = new WordTable();

            TableProperties tableProps = new TableProperties(
                new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 5
                }))
            {
                TablePositionProperties = new TablePositionProperties()
                {
                    TablePositionX = 256,
                    TablePositionY = 1097,
                    HorizontalAnchor = HorizontalAnchorValues.Page,
                    VerticalAnchor = VerticalAnchorValues.Page,
                    RightFromText = 180,
                    LeftFromText = 180
                }
            };

            table.AppendChild(tableProps);

            for (int i = 0; i < 5; i++)
            {
                TableRow tableRow = new TableRow(new TableRowProperties(
                    new TableRowHeight() { Val = (uint)(i == 4 ? 2600 : 2000) }));
                for (int j = 0; j < 2; j++)
                {
                    TableCell tableCell = new TableCell();

                    var p = new WordParagraph();
                    var pprops = new ParagraphProperties()
                    {
                        Justification = new Justification() { Val = JustificationValues.Center }
                    };
                    p.Append(pprops);

                    var r = new Run();
                    var rp = new RunProperties()
                    {
                        Italic = new Italic(),
                    };
                    var t = new Text(TitlePageTable[j][i]);

                    r.Append(rp);
                    r.Append(t);
                    p.Append(r);

                    tableCell.Append(new TableCellProperties(
                        new TableCellWidth { Width = "3000" })
                    {
                        TextDirection = new TextDirection() { Val = TextDirectionValues.BottomToTopLeftToRight },
                        TableCellVerticalAlignment = new TableCellVerticalAlignment()
                        { Val = TableVerticalAlignmentValues.Center },
                        TableCellWidth = new TableCellWidth() { Width = "529", Type = TableWidthUnitValues.Dxa },
                    });
                    tableCell.Append(p);

                    tableRow.Append(tableCell);
                }
                table.Append(tableRow);
            }

            Body.Append(table);
        }
        private void AddOrganizationHeader()
        {
            WordParagraph paragraph = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };

            paragraph.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(
                new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            Text text = new Text("ПРАВИТЕЛЬСТВО РОССИЙСКОЙ ФЕДЕРАЦИИ") { Space = SpaceProcessingModeValues.Preserve };

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddDocumentTitle()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "35" },

            };
            run.PrependChild(runProperties);

            var text = new Text(Documentation.ProjectName);

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddDocumentName()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.Append(runProperties);

            var text = new Text(Document.Name);

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddTitlePageTitle()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.Append(runProperties);

            var text = new Text("Лист утверждения");

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddDocumentNumber()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.Append(runProperties);

            var text = new Text(Documentation.ProjectCode);

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddTownAndYearParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text("Москва, 2019");

            run.Append(wordText);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        #endregion

        #region Second approval page methods
        private void AddApprovedParagraph()
        {
            WordParagraph paragraph = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Left },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };

            paragraph.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            Text text = new Text("Утвержден") { Space = SpaceProcessingModeValues.Preserve };

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);

            paragraph = new WordParagraph();
            pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Left },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Exact
                }
            };

            paragraph.Append(pp);

            run = new Run();
            runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Caps = new Caps(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            text = new Text(Documentation.ProjectCode);

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        private void AddListCountParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue(RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "25" }
            };
            run.Append(runProperties);

            var text = new Text("Листов <количесво листов>");

            run.Append(text);
            paragraph.Append(run);

            Body.Append(paragraph);
        }
        #endregion

        #region Table of contents methods
        private void AddTableOfContentsElement(SdtContentBlock sdtContentBlock, int depth,
            string name)
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new ParagraphStyleId() { Val = "31" })
            {
                Indentation = new Indentation() { FirstLine = "262", Left = "0" },
                Justification = new Justification() { Val = JustificationValues.Both }
            };

            RunFonts runFonts = new RunFonts()
            {
                Ascii = RenderSettings.FontFamily,
                HighAnsi = RenderSettings.FontFamily
            };
            pp.Append(runFonts);

            p.Append(pp);

            for (int i = 0; i < depth; i++)
            {
                p.Append(GetTabRun());
            }

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderSettings.FontFamily,
                Ascii = RenderSettings.FontFamily
            })
            {
                FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize }
            };
            if (depth == 0)
            {
                runProperties.Bold = new Bold();
            }
            Text text = new Text(name);

            run.Append(runProperties);
            run.Append(text);

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts() { HighAnsi = RenderSettings.FontFamily, Ascii = RenderSettings.FontFamily });
            run.Append(runProperties);
            run.Append(new PositionalTab()
            {
                Leader = AbsolutePositionTabLeaderCharValues.Dot,
                Alignment = AbsolutePositionTabAlignmentValues.Right,
                RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin
            });

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts() { HighAnsi = RenderSettings.FontFamily, Ascii = RenderSettings.FontFamily });
            run.Append(runProperties);
            text = new Text("0");
            run.Append(text);

            p.Append(run);

            sdtContentBlock.Append(p);
        }
        #endregion

        #region Paragraphs renders
        private void InsertSubparagraph(Subparagraph subparagraph, int depth)
        {
            Body.Append(RenderSubparagraph(subparagraph, depth));
        }
        private WordParagraph RenderSubparagraph(Subparagraph subparagraph, int depth)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Both },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                },
                Indentation = new Indentation() { Left = (500 * depth).ToString() }
            };
            paragraph.Append(pp);

            for (int i = 0; i < depth; i++)
            {
                paragraph.Append(GetTabRun());
            }

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue("Times New Roman"),
                Ascii = "Times New Roman"
            })
            {
                Color = new Color() { Val = RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize },

            };
            run.PrependChild(runProperties);

            var text = new Text(subparagraph.Text);

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
        private void InsertNumberedList(NumberedList numberedList, int depth)
        {
            var paragraphs = RenderNumberedList(numberedList, depth);

            foreach (var p in paragraphs)
            {
                Body.Append(p);
            }
        }
        private IEnumerable<WordParagraph> RenderNumberedList(NumberedList numberedList, int depth)
        {
            List<WordParagraph> wordParagraphs = new List<WordParagraph>();

            foreach (NumberedListElement element in numberedList.ListElements)
            {
                WordParagraph p = new WordParagraph();
                ParagraphProperties pp = new ParagraphProperties(new ParagraphStyleId() { Val = "a0" })
                {
                    Indentation = new Indentation() { Left = (500 * depth).ToString() }
                };
                NumberingProperties numberingProperties = new NumberingProperties()
                {
                    NumberingLevelReference = new NumberingLevelReference() { Val = 0 },
                    NumberingId = new NumberingId() { Val = 0 }
                };

                pp.Append(numberingProperties);
                p.Append(pp);

                Run run = new Run();
                RunProperties runProperties = new RunProperties(new RunFonts() { HighAnsi = "Times New Roman" })
                {
                    FontSize = new FontSize() { Val = "20" },
                    Color = new Color() { Val = RenderSettings.DefaultColor }
                };
                Text text = new Text(element.Text);

                run.Append(runProperties);
                run.Append(text);

                p.Append(run);

                wordParagraphs.Add(p);
            }

            return wordParagraphs;
        }
        private void InsertParagraphImage(ParagraphImage paragraphImage, int depth)
        {
            ImagePart imagePart = MainPart.AddImagePart(ImagePartType.Png);
            using (var ms = new MemoryStream(paragraphImage.ImageSource))
            {
                imagePart.FeedData(ms);
            }

            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Indentation = new Indentation() { Left = (500 * depth).ToString() }
            };

            p.Append(pp);
            Run run = new Run(GetDrawing(MainPart.GetIdOfPart(imagePart), depth));
            p.Append(run);

            Body.AppendChild(p);
        }
        private Drawing GetDrawing(string relationshipId, int depth)
        {
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            return element;
        }
        private void InsertTable(Models.Table table, int depth)
        {
            Body.Append(RenderTable(table, depth));
        }
        private WordTable RenderTable(Models.Table table, int depth)
        {
            WordTable wordTable = new WordTable();
            TableProperties tableProperties = new TableProperties(new TableBorders(
                new TopBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new BottomBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new LeftBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new RightBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new InsideHorizontalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                },
                new InsideVerticalBorder
                {
                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                    Size = 12
                }), new TableIndentation() { Width = (int)(depth * TabValue) });

            wordTable.Append(tableProperties);

            string[][] data = table.TableCells;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                TableRow tableRow = new TableRow();
                for (int j = 0; j < data[i].Length; j++)
                {
                    TableCell tableCell = new TableCell();

                    WordParagraph p = new WordParagraph();
                    ParagraphProperties pp = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

                    p.Append(pp);

                    Run run = new Run();
                    RunProperties rp = new RunProperties(new RunFonts() { HighAnsi = "Times New Roman", Ascii = "Times New Roman" })
                    {
                        Color = new Color() { Val = RenderSettings.DefaultColor },
                        FontSize = new FontSize() { Val = RenderSettings.DefaultTextSize }
                    };

                    run.Append(rp);

                    Text text = new Text(data[i][j]);
                    run.Append(text);
                    p.Append(run);

                    tableCell.Append(p);

                    tableRow.Append(tableCell);
                }

                wordTable.Append(tableRow);
            }

            return wordTable;
        }
        #endregion
    }
}
