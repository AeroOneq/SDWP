using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ApplicationLib.Word.Commands
{ 
    public class SoftwareEngineerSignatureCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }

        public SoftwareEngineerSignatureCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
        }

        public void Render()
        {
            Body body = WordDocument.MainDocumentPart.Document.Body;
            body.Append(new RightParagraphCommand("                     Исполнитель").GetElement());
            body.Append(new RightParagraphCommand("        <Введите свою должность>").GetElement());
            body.Append(new RightParagraphCommand("            /<Введите своё имя>/").GetElement());
            body.Append(new EmptyParagraphCommand().GetElement());
            body.Append(new RightParagraphCommand("    \"___\"_________________2019").GetElement());
        }
    }
}
