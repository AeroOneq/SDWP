using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ApplicationLib.Interfaces;

using Newtonsoft.Json;

using ApplicationLib.Views;


namespace ApplicationLib.Models
{
    public class NumberedList : IParagraphElement
    {
        #region Properties
        public List<NumberedListElement> ListElements { get; set; }

        [JsonIgnore]
        public Item ParentItem { get; }
        public string Hint { get; set; }
        public string Title { get; set; }
        #endregion

        public NumberedList(List<NumberedListElement> listElements, Item parentItem)
        {
            ListElements = listElements;
            ParentItem = parentItem;

            SetIndexes();
        }

        private void SetIndexes()
        {
            for (int i = 0; i < ListElements.Count; i++)
            {
                ListElements[i].Index = i.ToString();
            }
        }

        public void MoveItemUp(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < ListElements.Count)
            {
                if (itemIndex == 0)
                {
                    NumberedListElement firstItem = ListElements[0];
                    for (int i = 0; i < ListElements.Count - 1; i++)
                    {
                        ListElements[i] = ListElements[i + 1];
                    }
                    ListElements[ListElements.Count - 1] = firstItem;
                }
                else
                {
                    NumberedListElement temp = ListElements[itemIndex];
                    ListElements[itemIndex] = ListElements[itemIndex - 1];
                    ListElements[itemIndex - 1] = temp;
                }

                SetIndexes();
            }
        }

        public void MoveItemDown(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < ListElements.Count)
            {
                if (itemIndex == ListElements.Count - 1)
                {
                    NumberedListElement lastItem = ListElements[ListElements.Count - 1];
                    for (int i = ListElements.Count - 1; i > 0; i--)
                    {
                        ListElements[i] = ListElements[i - 1];
                    }
                    ListElements[0] = lastItem;
                }
                else
                {
                    NumberedListElement temp = ListElements[itemIndex];
                    ListElements[itemIndex] = ListElements[itemIndex + 1];
                    ListElements[itemIndex + 1] = temp;
                }
                SetIndexes();
            }
        }

        public void AddItem(int clickedItemIndex)
        {
            ListElements.Insert(clickedItemIndex + 1, new NumberedListElement(string.Empty));
            SetIndexes();
        }

        public void DeleteItem(int itemIndex)
        {
            ListElements.RemoveAt(itemIndex);
            SetIndexes();
        }

        public UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public UserControl GetEditView()
        {
            return new NumberedListEditView(this);
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }
    }
}
