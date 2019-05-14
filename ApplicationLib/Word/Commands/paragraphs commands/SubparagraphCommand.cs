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
    public class SubparagraphCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public Subparagraph Subparagraph { get; }
        public int Depth { get; }

        public SubparagraphCommand(WordprocessingDocument wordDocument, Subparagraph subparagraph,
            int depth)
        {
            WordDocument = wordDocument;
            Subparagraph = subparagraph;
            Depth = depth;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(RenderSubparagraph());
        }

        private WordParagraph RenderSubparagraph()
        {
            var paragraph = new WordParagraph();
            var pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Both },
                SpacingBetweenLines = new SpacingBetweenLines()
                {
                    Before = "100",
                    After = "100",
                    Line = "300",
                    LineRule = LineSpacingRuleValues.Exact
                },
                Indentation = new Indentation() { Left = (500 * Depth).ToString() }
            };
            paragraph.Append(pp);

            for (int i = 0; i < Depth; i++)
            {
                paragraph.Append(new TabCommand().GetElement());
            }

            var run = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = new StringValue("Times New Roman"),
                Ascii = "Times New Roman"
            })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize },

            };
            run.PrependChild(runProperties);

            var text = new Text(Subparagraph.Text);

            run.Append(text);
            paragraph.Append(new TabCommand().GetElement());
            paragraph.Append(run);

            return paragraph;
        }
    }
}
