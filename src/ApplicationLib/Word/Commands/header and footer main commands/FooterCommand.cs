using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;

using ApplicationLib.Word.Interfaces;

namespace ApplicationLib.Word.Commands
{
    public class FooterCommand : IWordCommand
    {
        #region Properties
        public WordprocessingDocument WordDocument { get; }

        public ICommandsContainer CommandsContainer { get; }
        #endregion

        public FooterCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()

        {
            var MainPart = WordDocument.MainDocumentPart;

            MainPart.DeleteParts(MainPart.FooterParts);

            FooterPart footerPart = MainPart.AddNewPart<FooterPart>();

            string footerID = MainPart.GetIdOfPart(footerPart);

            CreateFooterContent(footerPart);

            var sectionPropsList = MainPart.Document.Body.Descendants<SectionProperties>().ToList();
            var sectionProps = sectionPropsList[sectionPropsList.Count - 1];

            sectionProps.Append(new FooterReference() { Type = HeaderFooterValues.Default, Id = footerID });
        }

        public void CreateFooterContent(FooterPart footerPart)
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

            for (int i = 0; i < RenderData.Obj.RenderSettings.FooterTable.Length; i++)
            {
                TableRow tableRow = new TableRow();
                for (int j = 0; j < RenderData.Obj.RenderSettings.FooterTable[i].Length; j++)
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
                        Ascii = RenderData.Obj.RenderSettings.FontFamily,
                        HighAnsi = RenderData.Obj.RenderSettings.FontFamily
                    })
                    {
                        Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                        FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize }
                    };
                    Text text = new Text(RenderData.Obj.RenderSettings.FooterTable[i][j]);

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
    }
}
