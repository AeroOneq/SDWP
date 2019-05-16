using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Models;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using ApplicationLib.Word.Interfaces;

namespace ApplicationLib.Word.Commands
{
    class ParagraphImageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ParagraphImage ParagraphImage { get;  }
        public int Depth { get; }

        public ParagraphImageCommand(WordprocessingDocument document, ParagraphImage paragraphImage,
            int depth)
        {
            WordDocument = document;
            ParagraphImage = paragraphImage;
            Depth = depth;
        }

        public void Render()
        {
            ImagePart imagePart = WordDocument.MainDocumentPart.AddImagePart(ImagePartType.Png);
            using (var ms = new MemoryStream(ParagraphImage.ImageSource))
            {
                imagePart.FeedData(ms);
            }

            WordParagraph p = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Indentation = new Indentation() { Left = (500 * Depth).ToString() }
            };

            p.Append(pp);
            Run run = new Run(GetDrawing(WordDocument.MainDocumentPart.GetIdOfPart(imagePart)));
            p.Append(run);

            WordDocument.MainDocumentPart.Document.Body.AppendChild(p);
            WordDocument.MainDocumentPart.Document.Body.AppendChild(GetParagraphImageTitle());
        }
        private Drawing GetDrawing(string relationshipId)
        {
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            return element;
        }
        private WordParagraph GetParagraphImageTitle()
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

            var runNumber = new Run();
            var runProperties = new RunProperties(new RunFonts
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = (int.Parse(RenderData.Obj.
                    RenderSettings.DefaultTextSize) - 2).ToString() },
                Italic = new Italic()
            };
            runNumber.PrependChild(runProperties);

            var text = new Text("Рисунок (номер). ");

            runNumber.Append(text);

            var runName = new Run();
            var runNameProperties = new RunProperties(new RunFonts()
            {
                HighAnsi = RenderData.Obj.RenderSettings.FontFamily,
                Ascii = RenderData.Obj.RenderSettings.FontFamily
            })
            {
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize()
                {
                    Val = (int.Parse(RenderData.Obj.
                    RenderSettings.DefaultTextSize) - 2).ToString()
                },
            };

            var textName = new Text(ParagraphImage.Title);
            runName.Append(runNameProperties);
            runName.Append(textName);

            paragraph.Append(runNumber);
            paragraph.Append(runName);

            return paragraph;
        }
    }
}
