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
    public class ApprovalTableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; set; }

        public ApprovalTableCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetApprovalTable());
        }

        private WordTable GetApprovalTable()
        {
            WordTable table = new WordTable();

            TableProperties tableProperties = new TableProperties()
            {
                TableJustification = new TableJustification() { Val = TableRowAlignmentValues.Center },
            };

            table.AppendChild(tableProperties);

            TableRow tableRow = new TableRow();
            TableCell firstCell = new TableCell();

            firstCell.Append(new CenterParagraphCommand("              СОГЛАСОВАННО             ").GetElement());
            firstCell.Append(new CenterParagraphCommand("          Руководитель проекта         ").GetElement());
            firstCell.Append(new CenterParagraphCommand("      Должность должность должность    ").GetElement());
            firstCell.Append(new CenterParagraphCommand("      Должность должность должность    ").GetElement());
            firstCell.Append(new CenterParagraphCommand("                                       ").GetElement());
            firstCell.Append(new CenterParagraphCommand($"                     /{RenderData.Obj.Documentation.TeamLeadName}/")
                .GetElement());
            firstCell.Append(new CenterParagraphCommand("              \"___\"__________________").GetElement());

            tableRow.Append(firstCell);

            TableCell middleCell = new TableCell(new TableCellProperties()
            {
                TableCellWidth = new TableCellWidth() { Width = "1000" }
            });
            middleCell.Append(new EmptyParagraphCommand().GetElement());

            tableRow.Append(middleCell);

            TableCell secondCell = new TableCell();

            secondCell.Append(new CenterParagraphCommand("               УТВЕРЖДАЮ               ").GetElement());
            secondCell.Append(new CenterParagraphCommand("   Самый главный руководитель проекта  ").GetElement());
            secondCell.Append(new CenterParagraphCommand("      Должность должность должность    ").GetElement());
            secondCell.Append(new CenterParagraphCommand("      Должность должность должность    ").GetElement());
            secondCell.Append(new CenterParagraphCommand("                                       ").GetElement());
            secondCell.Append(new CenterParagraphCommand($"                     /{RenderData.Obj.Documentation.ManagerName}/")
                .GetElement());
            secondCell.Append(new CenterParagraphCommand("              \"___\"__________________").GetElement());

            tableRow.Append(secondCell);

            table.Append(tableRow);

            return table;
        }
    }
}
