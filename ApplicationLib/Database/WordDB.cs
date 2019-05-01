using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

using ApplicationLib.Models;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.InkML;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace ApplicationLib.Database
{
    public class WordDB
    {
        public Documentation Documentation { get; }
        public List<Models.Document> Documents { get; }
        public string FilePath { get; set; }

        #region Word render constants
        string[][] titlePageTable = new string[2][]
        {
            new string[5]{"Инв. № подл", "Подп и дата", "Взаим инв №", "Инв № дубл", "Подп и дата"}.Reverse().ToArray(),
            new string[5]{ "RU.17701729.04.03-01 ТЗ", "", "", "", "" }.Reverse().ToArray()
        };
        #endregion

        public WordDB(Documentation documentation, List<Models.Document> documents, string filePath)
        {
            Documentation = documentation;
            Documents = documents;
            FilePath = filePath;
        }

        public async Task CreateWordDocument()
        {
            await Task.Run(() =>
            {
                if (File.Exists("test.docx"))
                    File.Delete("test.docx");

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(
                    "test.docx", WordprocessingDocumentType.Document,
                    true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                    Body body = new Body();

                    CreateTitlePage(body);

                    mainPart.Document.Append(body);
                }
            });
        }

        private void CreateTitlePage(Body body)
        {
            AddTitlePageTable(body);
            AddOrganizationHeader(body);

            AddEmptyParagrahs(body, 1);

            AddApprovalTable(body);

            AddEmptyParagrahs(body, 2);

            AddDocumentTitle(body);
            AddDocumentName(body);
            AddTitlePageTitle(body);
            AddDocumentNumber(body);

            AddEmptyParagrahs(body, 3);

            AddSoftwateEngineerSignatureTable(body);

            AddEmptyParagrahs(body, 7);

            AddTownAndYearParagraph(body);
            body.Append(GetLastParagraphOfThePage());
        }
        private void AddEmptyParagrahs(Body body, int count)
        {
            for (int i = 0; i < count; i++)
            {
                body.Append(GetEmptyParagraph());
            }
        }
        private WordParagraph GetEmptyParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties() { Justification = new Justification() {
                Val = JustificationValues.Center } };
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

            var run = new Run(new Break() { Type = BreakValues.Page });
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text();

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }

        #region Title page methods
        private void AddSoftwateEngineerSignatureTable(Body body)
        {
            body.Append(GetRightParagraph("                     Исполнитель"));
            body.Append(GetRightParagraph("        <Введите свою должность>"));
            body.Append(GetRightParagraph("            /<Введите своё имя>/"));
            AddEmptyParagrahs(body, 1);
            body.Append(GetRightParagraph("    \"___\"_________________2019"));
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
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "27" },

            };
            run.PrependChild(runProperties);

            var wordText = new Text(text);

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
        private void AddApprovalTable(Body body)
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
            firstCell.Append(GetRightParagraph("                     /Имя руководителя/"));
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
            secondCell.Append(GetRightParagraph("                     /Имя руководителя/"));
            secondCell.Append(GetRightParagraph("              \"___\"__________________"));

            tableRow.Append(secondCell);

            table.Append(tableRow);
            body.Append(table);
        }

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
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "27" },

            };
            run.PrependChild(runProperties);

            var wordText = new Text(text);

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }

        private void AddTitlePageTable(Body body)
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
                TableRow tableRow = new TableRow(new TableRowProperties(new TableRowHeight() { Val = (uint)(i == 4 ? 2600 : 2000) }));
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
                    var t = new Text(titlePageTable[j][i]);

                    r.Append(rp);
                    r.Append(t);
                    p.Append(r);

                    tableCell.Append(new TableCellProperties(
                        new TableCellWidth { Width = "3000" })
                    {
                        TextDirection = new TextDirection() { Val = TextDirectionValues.BottomToTopLeftToRight },
                        TableCellVerticalAlignment = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center },
                        TableCellWidth = new TableCellWidth() { Width = "529", Type = TableWidthUnitValues.Dxa },
                    });
                    tableCell.Append(p);

                    tableRow.Append(tableCell);
                }
                table.Append(tableRow);
            }

            body.Append(table);

        }
        private void AddOrganizationHeader(Body body)
        {
            WordParagraph paragraph = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };

            paragraph.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            Text text = new Text("ПРАВИТЕЛЬСТВО РОССИЙСКОЙ ФЕДЕРАЦИИ") { Space = SpaceProcessingModeValues.Preserve };

            run.Append(text);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        private void AddDocumentTitle(Body body)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "35" },

            };
            run.PrependChild(runProperties);

            var text = new Text("НАСТОЛЬНОЕ ПРИЛОЖЕНИЕ ДЛЯ СОЗДАНИЯ ДОКУМЕНТАЦИИ ПРОГРАММНОГО ПРОДУКТА");

            run.Append(text);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        private void AddDocumentName(Body body)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "25" }
            };
            run.Append(runProperties);

            var text = new Text("Техническое задание");

            run.Append(text);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        private void AddTitlePageTitle(Body body)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "25" }
            };
            run.Append(runProperties);

            var text = new Text("Лист утверждения");

            run.Append(text);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        private void AddDocumentNumber(Body body)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "25" }
            };
            run.Append(runProperties);

            var text = new Text("RU.17701729.04.03-01 ТЗ 01-1-ЛУ");

            run.Append(text);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        private void AddTownAndYearParagraph(Body body)
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };
            paragraph.Append(pp);

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts { HighAnsi = new StringValue("Times New Roman") })
            {
                Color = new Color() { Val = "#000000" },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text("Москва, 2019");

            run.Append(wordText);
            paragraph.Append(run);

            body.Append(paragraph);
        }
        #endregion
    }
}
