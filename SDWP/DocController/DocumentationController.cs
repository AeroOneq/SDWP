using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace SDWP
{
    public class DocumentationController : IDocController
    {
        #region Propeties
        public Documentation Documentation { get; }
        public List<Document> Documents { get; }

        public Document CurrentDocument { get; set; }
        public Item CurrentItem { get; set; }
        public Item CurrentContentItem { get; set; }
        public List<Item> CurrentItemsList { get; set; }
        public List<IParagraphElement> CurrentParagraphsList { get; set; }
        #endregion

        public DocumentationController() { }
        public DocumentationController(Documentation documentation, List<Document> documents)
        {
            Documentation = documentation;
            Documents = documents;
        }

        public void UploadItem(Item selectedItem)
        {
            if (selectedItem.Items == null)
            {
                CurrentParagraphsList = selectedItem.Paragraphs;
            }
            else
            {
                CurrentItemsList = selectedItem.Items;
                CurrentItem = selectedItem;
            }
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
    }
}
