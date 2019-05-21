using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using ApplicationLib.Views;
using ApplicationLib.Models;

namespace ApplicationLib.Tests
{
    [TestFixture]
    public class TableTest
    {
        Table table;

        [SetUp]
        public void SetUpTests()
        {
            table = new Table(GetCells());
        }

        private string[][] GetCells()
        {
            return new string[5][]
            {
                new string[4] {"1", "2", "3", "4",},
                new string[4] {"a", "b", "c", "d",},
                new string[4] {"1", "2", "3", "4",},
                new string[4] {"1", "2", "3", "4",},
                new string[4] {"aa", "bb", "cc", "dd",}
            };
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestDeleteCol(int value)
        {
            int initialColCount = table.TableCells[0].Length;
            table.DeleteCol(value);

            if ((value < 0 || value >= initialColCount))
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 4)
                        Assert.Fail($"Out of range test failed {table.TableCells[i].Length}");
                }
            }
            else
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 3)
                        Assert.Fail(table.TableCells[i].Length.ToString());
                }
            }

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestDeleteRow(int value)
        {
            int initialRowCount = table.TableCells.Length;
            table.DeleteRow(value);

            if (value < 0 || value >= initialRowCount)
            {
                if (table.TableCells.Length != 5)
                    Assert.Fail("Out of range test failed");
            }
            else
            {
                if (table.TableCells.Length != 4)
                    Assert.Fail("Range test failed");
            }

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestAddDownRow(int rowIndex)
        {
            table.AddNewDownRow(rowIndex);

            if (rowIndex < 0 || rowIndex >= table.TableCells.Length)
            {
                if (table.TableCells.Length != 5)
                    Assert.Fail("Out of ranged test failed");
            }
            else
            {
                if (table.TableCells.Length != 6)
                    Assert.Fail("Range test failed");
            }

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestAddUpRow(int rowIndex)
        {
            table.AddNewUpRow(rowIndex);

            if (rowIndex < 0 || rowIndex >= table.TableCells.Length)
            {
                if (table.TableCells.Length != 5)
                    Assert.Fail("Out of ranged test failed");
            }
            else
            {
                if (table.TableCells.Length != 6)
                    Assert.Fail("Range test failed");
            }

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestAddLeftCol(int colNum)
        {
            int intialColCount = table.TableCells[0].Length;
            table.AddNewLeftCol(colNum);

            if (colNum < 0 || colNum >= intialColCount)
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 4)
                        Assert.Fail("Ranged test failed");
                }
            }
            else
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 5)
                        Assert.Fail("Ranged test failed");
                }
            }

            Assert.Pass();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        public void TestAddRightCol(int colNum)
        {
            int intialColCount = table.TableCells[0].Length;
            table.AddNewRightCol(colNum);

            if (colNum < 0 || colNum >= intialColCount)
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 4)
                        Assert.Fail("Ranged test failed");
                }
            }
            else
            {
                for (int i = 0; i < table.TableCells.Length; i++)
                {
                    if (table.TableCells[i].Length != 5)
                        Assert.Fail("Ranged test failed");
                }
            }

            Assert.Pass();
        }
    }
}
