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
    public class TitlePageCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; private set; }

        public TitlePageCommand(WordprocessingDocument document)
        {
            WordDocument = document;
            CommandsContainer = new CommandsContainer();
        }

        public void Render()
        {
            CreateCommandsList();

            CommandsContainer.Render();
        }

        private void CreateCommandsList()
        {
            CommandsContainer.Refresh();

            if (RenderData.Obj.RenderSettings.AddTitlePage)
                CommandsContainer.Add(new TitleTableCommand(WordDocument));

            CommandsContainer.Add(new OrganizationNameCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
            CommandsContainer.Add(new ApprovalTableCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 2));
            CommandsContainer.Add(new DocumentTitleCommand(WordDocument));
            CommandsContainer.Add(new DocumentNameCommand(WordDocument));
            CommandsContainer.Add(new TitlePageTitleCommand(WordDocument));
            CommandsContainer.Add(new DocumentNumberCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 3));
            CommandsContainer.Add(new SoftwareEngineerSignatureCommand(WordDocument));
            CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 5));
            CommandsContainer.Add(new TownAndYearCommand(WordDocument));
            CommandsContainer.Add(new EndOfPageCommand(WordDocument));
            CommandsContainer.Add(new SectionPtrCommand(WordDocument));
        }
    }
}
