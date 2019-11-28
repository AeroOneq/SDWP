using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;

using SDWP.Interfaces;

namespace SDWP
{
    public class DocumentationController : IDocController
    {
        #region Propeties
        public Documentation Documentation { get; private set; }
        public List<Document> Documents { get; private set; }

        public Document CurrentDocument { get; private set; }
        public Item CurrentItem { get; private set; }
        public Item CurrentContentItem { get; private set; }
        public List<Item> CurrentItemsList { get; private set; }
        public List<Paragraph> CurrentParagraphsList { get; private set; }
        #endregion

        public DocumentationController() { }

        public void UploadLocalDocumentation(LocalDocumentation localDocumentation)
        {
            Documentation = localDocumentation.Documentation;
            Documents = localDocumentation.Documents;
        }

        public void UploadCloudDocumentation(Documentation documentation, List<Document> documents)
        {
            Documentation = documentation;
            Documents = documents;
        }

        public void UploadItem(Item selectedItem)
        {
            if (selectedItem.Items == null)
            {
                CurrentParagraphsList = selectedItem.Paragraphs;
                CurrentContentItem = selectedItem;
            }
            else
            {
                CurrentItemsList = selectedItem.Items;
                CurrentItem = selectedItem;
            }
        }

        public void UploadParagraphs(Item selectedItem)
        {
            CurrentContentItem = selectedItem;
            CurrentParagraphsList = selectedItem.Paragraphs;
        }

        public void UploadDocument(Document document)
        {
            CurrentItemsList = document.Items;
            CurrentDocument = document;
        }

        public void GoToPreviousItem(Item currentItem)
        {
            CurrentItemsList = currentItem.ParentList;
            CurrentItem = currentItem.ParentItem;
        }

        public bool CanGoToPrevItem()
        {
            return CurrentItem != null && CurrentItem.ParentList != null;
        }

        /// <summary>
        /// Sets all references of properties to null
        /// </summary>
        public void Clear()
        {
            Documentation = null;
            Documents = null;
            CurrentContentItem = null;
            CurrentDocument = null;
            CurrentItem = null;
            CurrentItemsList = null;
            CurrentParagraphsList = null;
        }
    }
}
