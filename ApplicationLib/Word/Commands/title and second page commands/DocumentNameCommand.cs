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
    public class DocumentNameCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public DocumentNameCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetDocumentNameParagraph());
        }

        private WordParagraph GetDocumentNameParagraph()
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
            {
                HighAnsi = new StringValue(RenderData.Obj.RenderSettings.FontFamily),
                Ascii = new StringValue(RenderData.Obj.RenderSettings.FontFamily)
            })
            {
                Bold = new Bold(),
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.Append(runProperties);

            var text = new Text(RenderData.Obj.Document.Name);

            run.Append(text);
            paragraph.Append(run);
            return paragraph;
        }
    }
}
