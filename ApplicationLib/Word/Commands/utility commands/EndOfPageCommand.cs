using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class EndOfPageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public EndOfPageCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }


        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetLastParagraphOfThePage());
        }

        public WordParagraph GetLastParagraphOfThePage ()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };
            paragraph.Append(pp);

            var run = new Run(new LastRenderedPageBreak(), new Break() { Type = BreakValues.Page });
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue(
                RenderData.Obj.RenderSettings.FontFamily)
            })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "30" }
            };
            run.PrependChild(runProperties);

            var wordText = new Text();

            run.Append(wordText);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
