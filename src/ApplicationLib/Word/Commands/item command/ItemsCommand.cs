using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Word.Containers;
using ApplicationLib.Word.Interfaces;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace ApplicationLib.Word.Commands
{
    public class ItemsCommand : IWordCommand
    {
        public WordprocessingDocument WordDocument { get; }
        public ICommandsContainer CommandsContainer { get; private set; }

        public ItemsCommand(WordprocessingDocument wordDocument)
        {
            WordDocument = wordDocument;
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
            string index = "0";

            foreach (Item item in RenderData.Obj.Document.Items)
            {
                index = (int.Parse(index) + 1).ToString();
                AddCommandsToList(item, 0, index);
                CommandsContainer.Add(new EndOfPageCommand(WordDocument));
            }

            CommandsContainer.Add(new SectionPtrCommand(WordDocument));
        }

        private void AddCommandsToList(Item item, int depth, string index)
        {
            if (depth == 0)
                CommandsContainer.Add(new ItemHeaderCommand(WordDocument, index + ". " + item.Name));
            else
                CommandsContainer.Add(new ParagraphHeaderCommand(WordDocument, index + " " + item.Name, depth));

            if (item.Items != null)
            {
                string dopIndex = "0";
                foreach (Item i in item.Items)
                {
                    dopIndex = (int.Parse(dopIndex) + 1).ToString();
                    AddCommandsToList(i, depth + 1, index + "." + dopIndex);
                }
            }

            if (item.Paragraphs != null)
            {
                foreach (Models.Paragraph paragraph in item.Paragraphs)
                {
                    switch (paragraph.Type)
                    {
                        case "Table":
                            CommandsContainer.Add(new TableCommand(WordDocument,
                                paragraph.ParagraphElement as Models.Table, depth));
                            break;
                        case "Subparagraph":
                            CommandsContainer.Add(new SubparagraphCommand(WordDocument,
                                paragraph.ParagraphElement as Subparagraph, depth));
                            break;
                        case "NumberedList":
                            CommandsContainer.Add(new NumberedListCommand(WordDocument,
                                paragraph.ParagraphElement as NumberedList, depth));
                            break;
                        case "ParagraphImage":
                            CommandsContainer.Add(new ParagraphImageCommand(WordDocument,
                                paragraph.ParagraphElement as ParagraphImage, depth));
                            break;
                    }

                    CommandsContainer.Add(new EmptyParagraphsCommand(WordDocument, 1));
                }
            }
        }
    }
}
