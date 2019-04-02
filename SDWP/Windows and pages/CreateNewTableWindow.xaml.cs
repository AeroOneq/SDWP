using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ApplicationLib.Models;

namespace SDWP
{
    public partial class CreateNewTableWindow : Window
    {
        #region Propeties
        private string TableWidthTextBoxHint { get; } = "Строки...";
        private string TableHeightTextBoxHint { get; } = "Столбцы...";
        private string FilePathTextBoxHint { get; } = "Путь до файла...";

        private Item CurrentItem { get; }

        private TextBox TableWidthTextBox { get; }
        private TextBox TableHeightTextBox { get; }
        private TextBox FilePathTextBox { get; }
        private TextBox TableTitleTextBox { get; }
        #endregion

        public CreateNewTableWindow(Item currentItem)
        {
            InitializeComponent();

            CurrentItem = currentItem;

            TableWidthTextBox = tableWidthTextBox;
            TableHeightTextBox = tableHeightTextBox;
            FilePathTextBox = filePathTexxBox;
            TableTitleTextBox = tableTitleTextBox;
        }

        #region Event handlers
        private void TableWidthTextBoxTextGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(TableWidthTextBoxHint, sender as TextBox);
        }

        private void TableWidthTextBoxTextLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(TableWidthTextBoxHint, sender as TextBox);
        }

        private void TableHeightTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(TableHeightTextBoxHint, sender as TextBox);
        }

        private void TableHeightTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(TableHeightTextBoxHint, sender as TextBox);
        }

        private void FilePathTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(FilePathTextBoxHint, sender as TextBox);
        }

        private void FilePathTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(FilePathTextBoxHint, sender as TextBox);
        }

        private void OnTextBoxGotFocus(string textBoxHint, TextBox textBox)
        {
            if (textBox.Text == textBoxHint)
                textBox.Text = string.Empty;
        }
        private void OnTextBoxLostFocus(string textBoxHint, TextBox textBox)
        {
            if (textBox.Text == string.Empty)
                textBox.Text = textBoxHint;
        }

        private void CreateNewTable(object sender, RoutedEventArgs e)
        {
            if (FilePathTextBox.Text != FilePathTextBoxHint)
                CreateTableFromCSFile();
            else
                CreateEmptyTable();
        }

        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Table creation 
        private void CreateTableFromCSFile()
        {
#warning Create logic for creation table from file
            throw new NotImplementedException();
        }

        private void CreateEmptyTable()
        {
            int tableRowCount = 0, tableColCount = 0;
            string tableTitle = TableTitleTextBox.Text;

            if (CheckTableWidthAndHeight(ref tableRowCount, ref tableColCount) &&
                CheckTableTitle(tableTitle))
            {
                string[][] tableCells = GetEmptyTableCells(tableRowCount, tableColCount);
                Table table = new Table(tableCells, CurrentItem)
                {
                    Title = tableTitle
                };

                CurrentItem.Paragraphs.Add(table);

                DialogResult = true;
                Close();
            }
            else
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка при создании таблицы", "Неверно введены данные",
                    MessageBoxButton.OK);
            }
        }

        private string[][] GetEmptyTableCells(int rowNum, int colNum)
        {
            string[][] tableCells = new string[rowNum][];
            for (int i = 0; i < rowNum; i++)
            {
                tableCells[i] = new string[colNum];

                for (int j = 0; j < tableCells[i].Length; j++)
                    tableCells[i][j] = j.ToString();
            }

            return tableCells;
        }

        private bool CheckTableTitle(string title)
        {
            return !string.IsNullOrEmpty(title);
        }

        /// <summary>
        /// Checks the correctness of the table width and height and id everything is OK sets the w and h ref variables
        /// the values of table width and height
        /// </summary>
        private bool CheckTableWidthAndHeight(ref int rows, ref int cols)
        {
            bool result = int.TryParse(TableWidthTextBox.Text, out rows) | int.TryParse(TableHeightTextBox.Text, out cols);
            result = result && !(rows <= 0 || rows >= 50) && !(cols <= 0 || cols >= 50);
            return result;
        }
        #endregion
    }
}
