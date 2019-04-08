using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using ApplicationLib.Models;

namespace ApplicationLib.Tests
{
    [TestFixture]
    class NumberedListTests
    {
        private NumberedListElement fixedElement;
        private NumberedList numberedList;

        [SetUp]
        public void SetUpTest()
        {
            numberedList = new NumberedList(new List<NumberedListElement>()
            {
                new NumberedListElement("first"),
                new NumberedListElement("second"),
                new NumberedListElement("third"),
                new NumberedListElement("fourth"),
                new NumberedListElement("fifth")
            });
        }

        [TestCase(0, ExpectedResult = 4)]
        [TestCase(1, ExpectedResult = 0)]
        [TestCase(2, ExpectedResult = 1)]
        [TestCase(3, ExpectedResult = 2)]
        [TestCase(4, ExpectedResult = 3)]
        public int TestMoveItemUp(int itemIndex)
        {
            fixedElement = numberedList.ListElements[itemIndex];
            numberedList.MoveItemUp(itemIndex);

            return numberedList.ListElements.FindIndex(item => item.Equals(fixedElement));
        }

        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 3)]
        [TestCase(3, ExpectedResult = 4)]
        [TestCase(4, ExpectedResult = 0)]
        public int TestMoveDownItem(int itemIndex)
        {
            fixedElement = numberedList.ListElements[itemIndex];
            numberedList.MoveItemDown(itemIndex);

            return numberedList.ListElements.FindIndex(item => item.Equals(fixedElement));
        }

        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 3)]
        [TestCase(3, ExpectedResult = 4)]
        [TestCase(4, ExpectedResult = 5)]
        public int TestAddNewItem(int selectedItemIndex)
        {
            fixedElement = new NumberedListElement(string.Empty);
            numberedList.AddItem(selectedItemIndex);

            return numberedList.ListElements.FindIndex(item => item.Text == fixedElement.Text);
        }

        [TearDown]
        public void TearDownTest()
        {
            fixedElement = null;
            numberedList = null;
        }
    }
}
