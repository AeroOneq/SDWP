using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using ApplicationLib.Word.Containers;

namespace ApplicationLib.Word.Commands
{
    internal class SecondPageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; private set; }

        public SecondPageCommand(WordprocessingDocument wordDocument)
        {
            CommandsContainer = new CommandsContainer();
            WordDocument = wordDocument;
        }

        public void Render()
        {
            CreateCommandsList();

            CommandsContainer.Render();
        }

        private void CreateCommandsList()
        {
            CommandsContainer.Refresh();

            CommandsContainer.Add(new TitleTableCommand(WordDocument));
            CommandsContainer.Add(new ApprovedParagraphCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 5));
            CommandsContainer.Add(new DocumentTitleCommand(WordDocument));
            CommandsContainer.Add(new DocumentNameCommand(WordDocument));
            CommandsContainer.Add(new PagesCountCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
            CommandsContainer.Add(new EndOfPageCommand(WordDocument));
            CommandsContainer.Add(new SectionPtrCommand(WordDocument));
        }
    }
}
