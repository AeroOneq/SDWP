using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ApplicationLib.Word.Interfaces;

namespace ApplicationLib.Word.Commands
{
    class HeaderCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; }

        public HeaderCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            var MainPart = WordDocument.MainDocumentPart;
            MainPart.DeleteParts(MainPart.HeaderParts);

            HeaderPart headerPart = MainPart.AddNewPart<HeaderPart>();
            string headerID = MainPart.GetIdOfPart(headerPart);

            CreateHeaderContent(headerPart);

            var sectionPropsList = MainPart.Document.Body.Descendants<SectionProperties>().ToList();
            var sectionProps = sectionPropsList[sectionPropsList.Count - 1];

            sectionProps.Append(new HeaderReference() { Type = HeaderFooterValues.Default, Id = headerID });
        }

        private void CreateHeaderContent(HeaderPart headerPart)
        {
            Header header = new Header() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            header.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            header.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            header.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            header.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            header.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            header.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            header.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            header.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            header.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            header.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            header.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            header.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            header.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            header.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            header.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            header.Append(GetHeaderContent());
            headerPart.Header = header;
        }
        private SdtBlock GetHeaderContent()
        {
            return new SdtBlock(
                            new SdtProperties(
                                new SdtId() { Val = 317275692 },
                                new SdtContentDocPartObject(
                                    new DocPartGallery() { Val = "Page Numbers (Top of Page)" },
                                    new DocPartUnique())),
                            new SdtContentBlock(
                                new WordParagraph(
                                    new ParagraphProperties(
                                        new ParagraphStyleId() { Val = "Header" },
                                        new Justification() { Val = JustificationValues.Center }),
                                    new SimpleField(
                                        new Run(
                                            new RunProperties(
                                                new NoProof(),
                                                new SpacingBetweenLines()
                                                {
                                                    After = "100",
                                                    Before = "100",
                                                    Line = "200"
                                                }),
                                            new Text("1"))
                                        { RsidRunAddition = "001F06F5" })
                                    { Instruction = " PAGE   \\* MERGEFORMAT " })
                                { RsidParagraphAddition = "00F1559F", RsidRunAdditionDefault = "00BB29E4" },
                                new WordParagraph(
                                    new ParagraphProperties(
                                        new Caps(),
                                        new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                                        new FontSize() { Val = RenderData.Obj.RenderSettings.DefaultTextSize },
                                        new Justification() { Val = JustificationValues.Center },
                                        new SpacingBetweenLines()
                                        {
                                            After = "100",
                                            Before = "0",
                                            Line = "200"
                                        }),
                                    new Run(
                                        new RunProperties(new RunFonts()
                                        {
                                            Ascii = RenderData.Obj.RenderSettings.FontFamily,
                                            HighAnsi = RenderData.Obj.RenderSettings.FontFamily
                                        }),
                                        new Text("RU.17701729.04.03-01 ТЗ")))));
        }
    }
}
