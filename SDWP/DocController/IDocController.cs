using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

namespace SDWP
{
    public interface IDocController
    {
        /// <summary>
        /// Current documentation
        /// </summary>
        Documentation Documentation { get; }
        /// <summary>
        /// List of documetns of this documentation
        /// </summary>
        List<Document> Documents { get; }

        /// <summary>
        /// The item, which items are now uploaded
        /// </summary>
        Item CurrentItem { get; set; }
        /// <summary>
        /// An item, which content os now uploaded
        /// </summary>
        Item CurrentContentItem { get; set; }
        /// <summary>
        /// The list where the CurrentItem is
        /// </summary>
        List<Item> CurrentItemsList { get; set; }
        /// <summary>
        /// The list of paragraphs which is now uploaded to UI
        /// </summary>
        List<IParagraphElement> CurrentParagraphsList { get; set; }

        /// <summary>
        /// Changes the state of this controller by uploading the selected doc
        /// </summary>
        void UploadDocument(Document document);
        /// <summary>
        /// Changes the state of this controller by uploading the content or list of items of
        /// selected item
        /// </summary>
        void UploadItem(Item selectedItem);
        /// <summary>
        /// Decides whether it is possible to go to the previous item
        /// </summary>
        bool CanGoToPrevItem();
        /// <summary>
        /// Changes the state of this controller when the user wants to get to previous item
        /// </summary>
        /// <param name="currentItem">CURRENT (the item with which user is working now) item</param>
        void GoToPreviousItem(Item currentItem);
    }
}
