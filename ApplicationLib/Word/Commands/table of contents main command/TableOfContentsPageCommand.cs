using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using ApplicationLib.Word.Containers;

namespace ApplicationLib.Word.Commands
{
    public class TableOfContentsPageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; private set; }

        public TableOfContentsPageCommand(WordprocessingDocument document)
        {
            CommandsContainer = new CommandsContainer();
            WordDocument = document;
        }

        public void Render()
        {
            CreateCommandsList();

            CommandsContainer.Render();
        }

        private void CreateCommandsList()
        {
            CommandsContainer.Refresh();

            CommandsContainer.Add(new ContentsTableCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
            CommandsContainer.Add(new EndOfPageCommand(WordDocument));
        }
    }
}
