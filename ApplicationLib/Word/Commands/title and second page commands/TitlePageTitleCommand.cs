using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ApplicationLib.Word.Commands
{
    class TitlePageTitleCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public TitlePageTitleCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetTitlePageTitleParagraph());
        }

        private WordParagraph GetTitlePageTitleParagraph()
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

            var text = new Text("Лист утверждения");

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
