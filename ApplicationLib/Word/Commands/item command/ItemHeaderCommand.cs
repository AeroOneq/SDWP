using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Word.Interfaces;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ApplicationLib.Word.Commands
{
    public class ItemHeaderCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public string Name { get; }

        public ItemHeaderCommand(WordprocessingDocument wordDocument, string name)
        {
            WordDocument = wordDocument;
            Name = name;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetSectionHeadParagraph());
        }

        private WordParagraph GetSectionHeadParagraph()
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new Justification() { Val = JustificationValues.Center });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize },
                Caps = new Caps(),
                Bold = new Bold()
            };

            run.Append(runProperties);

            Text text = new Text(Name);

            run.Append(text);
            p.Append(run);

            return p;
        }
    }
}
