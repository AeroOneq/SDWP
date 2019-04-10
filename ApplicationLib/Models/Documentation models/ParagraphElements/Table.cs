using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ApplicationLib.Interfaces;
using ApplicationLib.Views;

using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    public class Table : ParagraphElement
    {
        #region Properties
        public string[][] TableCells { get; set; }
        #endregion

        public Table(string[][] tableCells)
        {
            TableCells = tableCells;
        }

        public void DeleteCol(int colNum)
        {
            if (colNum >= 0 && TableCells.Length > 0 && colNum < TableCells[0].Length)
                for (int i = 0; i < TableCells.Length; i++)
                {
                    string[] newRow = new string[TableCells[i].Length - 1];

                    int currIndex = 0;
                    for (int j = 0; j < TableCells[i].Length; j++)
                    {
                        if (j != colNum)
                        {
                            newRow[currIndex] = TableCells[i][j];
                            currIndex++;
                        }
                    }

                    TableCells[i] = newRow;
                }
        }

        public void DeleteRow(int rowNum)
        {
            if (rowNum >= 0 && rowNum < TableCells.Length)
            {
                string[][] newCells = new string[TableCells.Length - 1][];

                int currIndex = 0;
                for (int i = 0; i < TableCells.Length; i++)
                {
                    if (i != rowNum)
                    {
                        newCells[currIndex] = TableCells[i];
                        currIndex++;
                    }
                }

                TableCells = newCells;
            }
        }

        public void AddNewDownRow(int rowNum)
        {
            if (rowNum >= 0 && TableCells.Length > 0 && rowNum < TableCells.Length)
            {
                string[][] newCells = new string[TableCells.Length + 1][];

                int currIndex = 0;
                for (int i = 0; i < newCells.Length; i++)
                {
                    newCells[i] = new string[TableCells[0].Length];

                    if (i != rowNum + 1)
                    {
                        for (int j = 0; j < newCells[i].Length; j++)
                        {
                            newCells[i][j] = TableCells[currIndex][j];
                        }
                        currIndex++;
                    }
                }

                TableCells = newCells;
            }
        }

        public void AddNewUpRow(int rowNum)
        {
            if (rowNum >= 0 && TableCells.Length > 0 && rowNum < TableCells.Length)
            {
                string[][] newCells = new string[TableCells.Length + 1][];

                int currIndex = 0;
                for (int i = 0; i < newCells.Length; i++)
                {
                    newCells[i] = new string[TableCells[0].Length];

                    if (i != rowNum)
                    {
                        for (int j = 0; j < newCells[i].Length; j++)
                        {
                            newCells[i][j] = TableCells[currIndex][j];
                        }
                        currIndex++;
                    }
                }

                TableCells = newCells;
            }
        }

        public void AddNewLeftCol(int colNum)
        {
            if (colNum >= 0 && TableCells.Length > 0 && colNum < TableCells[0].Length)
            {
                string[][] newCells = new string[TableCells.Length][];

                for (int i = 0; i < newCells.Length; i++)
                {
                    newCells[i] = new string[TableCells[i].Length + 1];

                    int currIndex = 0;
                    for (int j = 0; j < newCells[i].Length; j++)
                    {
                        if (j != colNum)
                        {
                            newCells[i][j] = TableCells[i][currIndex];
                            currIndex++;
                        }
                    }
                }

                TableCells = newCells;
            }
        }

        public void AddNewRightCol(int colNum)
        {
            if (colNum >= 0 && TableCells.Length > 0 && colNum < TableCells[0].Length)
            {
                string[][] newCells = new string[TableCells.Length][];

                for (int i = 0; i < newCells.Length; i++)
                {
                    newCells[i] = new string[TableCells[i].Length + 1];

                    int currIndex = 0;
                    for (int j = 0; j < newCells[i].Length; j++)
                    {
                        if (j != colNum + 1)
                        {
                            newCells[i][j] = TableCells[i][currIndex];
                            currIndex++;
                        }
                    }
                }

                TableCells = newCells;
            }
        }

        public override UserControl GetWatchView()
        {
            throw new NotImplementedException();
        }

        public override UserControl GetEditView()
        {
            return new TableEditView(this);
        }
    }
}
