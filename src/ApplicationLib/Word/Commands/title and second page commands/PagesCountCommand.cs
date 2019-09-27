using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class PagesCountCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public PagesCountCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetListCountParagraph());
        }

        private WordParagraph GetListCountParagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
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
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "25" }
            };
            run.Append(runProperties);

            var text = new Text("Листов <количесво листов>");

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
