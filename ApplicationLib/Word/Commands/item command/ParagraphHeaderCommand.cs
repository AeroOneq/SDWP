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
    class ParagraphHeaderCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public string Name { get;  }
        public int Depth { get; }

        public ParagraphHeaderCommand(WordprocessingDocument document, string name, int depth)
        {
            WordDocument = document;
            Name = name;
            Depth = depth;
        }
        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetParagraphHeader());
        }

        private WordParagraph GetParagraphHeader()
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Left },
                new Indentation() { Left = (RenderData.Obj.RenderSettings.TabValue * Depth).ToString() });

            p.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            { HighAnsi = RenderData.Obj.RenderSettings.FontFamily, Ascii = RenderData.Obj.RenderSettings.FontFamily })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize },
                Bold = new Bold()
            };

            Text text = new Text(Name);

            run.Append(runProperties);
            run.Append(text);
            p.Append(run);

            return p;
        }
    }
}
