using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    class NumberedListCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        private int Depth { get; }
        private NumberedList NumberedList { get; }

        public NumberedListCommand(WordprocessingDocument wordDocument, NumberedList numberedList,
            int depth)
        {
            Depth = depth;
            WordDocument = wordDocument;
            NumberedList = numberedList;
        }

        public void Render()
        {
            var paragraphs = RenderNumberedList();

            foreach (var p in paragraphs)
            {
                WordDocument.MainDocumentPart.Document.Body.Append(p);
            }
        }
        private IEnumerable<WordParagraph> RenderNumberedList()
        {
            List<WordParagraph> wordParagraphs = new List<WordParagraph>();

            int index = 1;
            foreach (NumberedListElement element in NumberedList.ListElements)
            {
                WordParagraph p = new WordParagraph();
                ParagraphProperties pp = new ParagraphProperties(new ParagraphStyleId() { Val = "a0" })
                {
                    Indentation = new Indentation() { Left = (500 * Depth).ToString() },
                    SpacingBetweenLines = new SpacingBetweenLines()
                    {
                        Before = "100",
                        After = "100",
                        Line = "200",
                        LineRule = LineSpacingRuleValues.Exact
                    },
                };
                NumberingProperties numberingProperties = new NumberingProperties()
                {
                    NumberingLevelReference = new NumberingLevelReference() { Val = 0 },
                    NumberingId = new NumberingId() { Val = 0 }
                };

                pp.Append(numberingProperties);
                p.Append(pp);

                Run run = new Run();
                RunProperties runProperties = new RunProperties(new RunFonts() { HighAnsi = "Times New Roman" })
                {
                    FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize },
                    Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor }
                };
                Text text = new Text(index + ") " + element.Text);

                run.Append(runProperties);
                run.Append(text);

                p.Append(run);

                wordParagraphs.Add(p);
                index++;
            }

            return wordParagraphs;
        }
    }
}
