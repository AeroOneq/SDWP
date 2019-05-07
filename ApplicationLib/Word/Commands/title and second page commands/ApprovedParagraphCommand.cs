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
    class ApprovedParagraphCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public ApprovedParagraphCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetApprovedParagraph());
            WordDocument.MainDocumentPart.Document.Body.Append(GetNumberParagraph());
        }

        private WordParagraph GetApprovedParagraph()
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
            RunProperties runProperties = new RunProperties(
                new RunFonts
                {
                    HighAnsi = new StringValue(RenderData.Obj.RenderSettings.FontFamily)
                })
            {
                Caps = new Caps(),
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            Text text = new Text("Утвержден") { Space = SpaceProcessingModeValues.Preserve };

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
        private WordParagraph GetNumberParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
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

            var run = new Run();
            var runProperties = new RunProperties(
                new RunFonts
                { HighAnsi = new StringValue(RenderData.Obj.RenderSettings.FontFamily) })
            {
                Caps = new Caps(),
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            var text = new Text(RenderData.Obj.Documentation.ProjectCode);

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
