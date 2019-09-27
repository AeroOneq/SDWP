using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class TitleTableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public TitleTableCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetTitlePageTable());
        }

        private WordTable GetTitlePageTable()
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
                    var t = new Text(RenderData.Obj.RenderSettings.TitlePageTable[j][i]);

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

            return table; 
        }
    }
}
