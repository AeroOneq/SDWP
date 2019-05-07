﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

using ApplicationLib.Word.Interfaces;
using ApplicationLib.Models;

using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class TableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private Models.Table Table { get; }
        private int Depth { get; }

        public TableCommand(WordprocessingDocument wordDocument, Models.Table table,
            int depth)
        {
            Depth = depth;
            WordDocument = wordDocument;
            Table = table;
        }

        public void Render()
        {
            WordTable wordTable = RenderTable();
            WordDocument.MainDocumentPart.Document.Body.Append(wordTable);
        }

        private WordTable RenderTable()
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
                }), new TableIndentation() { Width = (int)(Depth * RenderData.Obj.RenderSettings.TabValue) });

            wordTable.Append(tableProperties);

            string[][] data = Table.TableCells;
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
                        Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                        FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize }
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
    }
}
