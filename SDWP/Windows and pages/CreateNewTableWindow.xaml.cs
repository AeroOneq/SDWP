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
using System.IO;

using Microsoft.Win32;

using FileLib.Interfaces;
using FileLib.FileParsers;

using SDWP.Factories;
using SDWP.Exceptions;
using SDWP.Interfaces;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

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

        private ISdwpAbstractFactory AbstractFactory { get; set; }
        private IFileParser FileParser { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        public CreateNewTableWindow(Item currentItem)
        {
            InitializeComponent();

            CurrentItem = currentItem;

            TableWidthTextBox = tableWidthTextBox;
            TableHeightTextBox = tableHeightTextBox;
            FilePathTextBox = filePathTexxBox;
            TableTitleTextBox = tableTitleTextBox;

            GetServices();
        }

        private void GetServices()
        {
            AbstractFactory = new SdwpAbstractFactory();

            FileParser = AbstractFactory.GetFileParser();
            ExceptionHandler = AbstractFactory.GetExceptionHandler(Dispatcher);
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
                CreateTableFromCSFile(FilePathTextBox.Text);
            else
                CreateEmptyTable();
        }

        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectAssembly(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "(*dll)|*dll",
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Выберете DLL файл для сканирования",
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }
        #endregion

        #region Table creation 
        private void CreateTableFromCSFile(string filePath)
        {
            try
            {
                Table[] tables = FileParser.GetAssemblyTables(filePath);

                for (int i = 0; i<tables.Length; i++)
                {
                    AddNewTable(tables[i]);
                }

                DialogResult = true;
                Close();
            }
            catch (IOException ex)
            {
                DialogResult = false;
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                DialogResult = false;
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private void AddNewTable(Table table)
        {
            Paragraph paragraph = new Paragraph(typeof(Table).Name, table);
            (paragraph as IParentableParagraph).SetParents(CurrentItem, CurrentItem.Paragraphs);

            CurrentItem.Paragraphs.Add(paragraph);
        }

        private void CreateEmptyTable()
        {
            int tableRowCount = 0, tableColCount = 0;
            string tableTitle = TableTitleTextBox.Text;

            if (CheckTableWidthAndHeight(ref tableRowCount, ref tableColCount) &&
                CheckTableTitle(tableTitle))
            {
                string[][] tableCells = GetEmptyTableCells(tableRowCount, tableColCount);
                Table table = new Table(tableCells)
                {
                    Title = tableTitle
                };

                AddNewTable(table);
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
