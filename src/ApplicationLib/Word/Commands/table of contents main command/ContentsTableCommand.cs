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
using ApplicationLib.Models;

namespace ApplicationLib.Word.Commands
{
    public class ContentsTableCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public ContentsTableCommand(WordprocessingDocument document)
        {
            WordDocument = document;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetTableOfContents());
        }

        private SdtBlock GetTableOfContents()
        {
            SdtBlock tableOfContents = new SdtBlock();
            RunProperties tocRpr = new RunProperties(new RunFonts()
            { HighAnsi = RenderData.Obj.RenderSettings.FontFamily },
                new Color() { Val = "auto" }, new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize });
            SdtContentDocPartObject sdtContentDocPartObject = new SdtContentDocPartObject(
                new DocPartGallery() { Val = "Table of Contents" }, new DocPartUnique());

            SdtProperties sdtProperties = new SdtProperties(tocRpr, sdtContentDocPartObject);

            tableOfContents.Append(sdtProperties);
            tableOfContents.Append(new SdtEndCharProperties());

            SdtContentBlock sdtContentBlock = new SdtContentBlock();

            WordParagraph p = new WordParagraph();
            ParagraphProperties ppr = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center }
            };

            p.Append(ppr);

            RunProperties rpr = new RunProperties(new RunFonts()
            {
                Ascii = RenderData.Obj.RenderSettings.FontFamily,
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily
            })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize }
            };

            Run run = new Run();
            run.Append(rpr);

            Text text = new Text("Содержание");
            run.Append(text);

            p.Append(run);

            sdtContentBlock.Append(p);

            string index = "0";
            foreach (Item item in RenderData.Obj.Document.Items)
            {
                index = (int.Parse(index) + 1).ToString();
                UploadItemsToTableOfContents(item, sdtContentBlock, 0, index);
            }

            tableOfContents.Append(sdtContentBlock);

            return tableOfContents;
        }

        private void UploadItemsToTableOfContents(Item item, SdtContentBlock sdtContentBlock,
            int depth, string index)
        {
            AddTableOfContentsElement(sdtContentBlock, depth, index + ". " + item.Name);

            string dopIndex = "0";
            if (item.Items != null)
                foreach (Item i in item.Items)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    UploadItemsToTableOfContents(i, sdtContentBlock, depth + 1, index + "." + dopIndex);
                }
        }

        private void AddTableOfContentsElement(SdtContentBlock sdtContentBlock, int depth,
           string name)
        {
            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties(new ParagraphStyleId() { Val = "31" })
            {
                Indentation = new Indentation() { FirstLine = "262", Left = "0" },
                Justification = new Justification() { Val = JustificationValues.Both }
            };

            RunFonts runFonts = new RunFonts()
            {
                Ascii = RenderData.Obj.RenderSettings.FontFamily,
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily
            };
            pp.Append(runFonts);

            p.Append(pp);

            for (int i = 0; i < depth; i++)
            {
                p.Append(new TabCommand().GetElement());
            }

            Run run = new Run();
            RunProperties runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            })
            {
                FontSize = new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize }
            };
            if (depth == 0)
            {
                runProperties.Bold = new Bold();
            }
            Text text = new Text(name);

            run.Append(runProperties);
            run.Append(text);

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            });
            run.Append(runProperties);
            run.Append(new PositionalTab()
            {
                Leader = AbsolutePositionTabLeaderCharValues.Dot,
                Alignment = AbsolutePositionTabAlignmentValues.Right,
                RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin
            });

            p.Append(run);

            run = new Run();
            runProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            });
            run.Append(runProperties);
            text = new Text("0");
            run.Append(text);

            p.Append(run);

            sdtContentBlock.Append(p);
        }
    }
}
