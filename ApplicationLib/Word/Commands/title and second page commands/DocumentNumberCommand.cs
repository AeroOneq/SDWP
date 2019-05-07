using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    class DocumentNumberCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public DocumentNumberCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetDocumentNumberParagraph());
        }

        private WordParagraph GetDocumentNumberParagraph()
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
            var runProperties = new RunProperties(
                new RunFonts
                {
                    HighAnsi = new StringValue(RenderData.Obj.RenderSettings.FontFamily)
                })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.Append(runProperties);

            var text = new Text(RenderData.Obj.Documentation.ProjectCode);

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
