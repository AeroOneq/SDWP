using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

using ApplicationLib.Word.Interfaces;
using ApplicationLib.Word.Containers;
using ApplicationLib.Word.Commands;

namespace ApplicationLib.Word
{
    public class WordRenderer : IWordRenderer
    {
        public IMainCommandsContainer RenderCommands { get; set; }
        public IMainCommandsContainer MainCommandsContainer { get; set; }
        private WordprocessingDocument WordDocument { get; set; }

        public WordRenderer()
        {
            MainCommandsContainer = new MainCommandsContainer();
        }

        public void SetRenderParams(RenderSettings renderSettings, Models.Document document,
            Documentation documentation)
        {
            RenderData.UpdateData(renderSettings, document, documentation);
        }

        #region IDocumentRenderer
        public async Task Render()
        {
            await Task.Run(() =>
            {
                string filePath = Path.Combine(RenderData.Obj.RenderSettings.FolderPath,
                    RenderData.Obj.Document.Name + ".docx");

                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (WordDocument = WordprocessingDocument.Create(
                    filePath, WordprocessingDocumentType.Document, true))
                {
                    WordDocument.AddMainDocumentPart();
                    WordDocument.MainDocumentPart.Document = new WordDocument();
                    WordDocument.MainDocumentPart.Document.Body = new Body();

                    SetDocumentCommands();

                    MainCommandsContainer.Render();
                }

                using (WordDocument = WordprocessingDocument.Open(filePath, true))
                {
                    SetFooterAndHeaderCommands();

                    MainCommandsContainer.Render();
                }
            });
        }
        #endregion

        private void SetFooterAndHeaderCommands()
        {
            MainCommandsContainer.Refresh();

            if (RenderData.Obj.RenderSettings.AddFooter)
                MainCommandsContainer.Add(new FooterCommand(WordDocument));

            if (RenderData.Obj.RenderSettings.AddHeader)
                MainCommandsContainer.Add(new HeaderCommand(WordDocument));
        }

        private void SetDocumentCommands()
        {
            MainCommandsContainer.Refresh();

            if (RenderData.Obj.RenderSettings.AddTitlePage)
                MainCommandsContainer.Add(new TitlePageCommand(WordDocument));

            if (RenderData.Obj.RenderSettings.AddSecondPage)
                MainCommandsContainer.Add(new SecondPageCommand(WordDocument));

            MainCommandsContainer.Add(new TableOfContentsPageCommand(WordDocument));
            MainCommandsContainer.Add(new ItemsCommand(WordDocument));
        }
    }
}
