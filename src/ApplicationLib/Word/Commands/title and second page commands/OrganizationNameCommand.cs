using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Commands
{
    public class OrganizationNameCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public OrganizationNameCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            WordDocument.MainDocumentPart.Document.Body.Append(GetOrganizationHeaderParagraph());
        }

        private WordParagraph GetOrganizationHeaderParagraph()
        {
            WordParagraph paragraph = new WordParagraph();
            ParagraphProperties pp = new ParagraphProperties()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
            };

            paragraph.Append(pp);

            Run run = new Run();
            RunProperties runProperties = new RunProperties(
                new RunFonts { HighAnsi = new StringValue(RenderData.Obj.RenderSettings.FontFamily) })
            {
                Bold = new Bold(),
                Caps = new Caps(),
                Color = new Color() { Val = RenderData.Obj.RenderSettings.DefaultColor },
                FontSize = new FontSize() { Val = "27" }
            };
            run.PrependChild(runProperties);

            Text text = new Text("ПРАВИТЕЛЬСТВО РОССИЙСКОЙ ФЕДЕРАЦИИ") { Space = SpaceProcessingModeValues.Preserve };

            run.Append(text);
            paragraph.Append(run);

            return paragraph;
        }
    }
}
