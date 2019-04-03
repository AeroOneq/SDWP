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
using System.Windows.Navigation;
using System.Windows.Shapes;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Views
{
    public partial class TableEditView : UserControl, IParagraphEditView
    {
        private Table CurrentTable { get; }
        private DataGrid TableDataGrid { get; }
        private HintControl HintControl { get; }
        private ParagraphElementSettings ParagraphSettings { get; }

        #region IParagraphEditView
        public Action RefreshParagraphsUI { get; set; }
        public List<IParagraphElement> ParentList { get; set; }
        #endregion

        public TableEditView(Table table)
        {
            InitializeComponent();

            CurrentTable = table;
            TableDataGrid = tableDataGrid;
            HintControl = hintControl;
            ParagraphSettings = paragraphsSettings;

            SetParagraphSettingsEvents();
            HintControl.SetBinding(CurrentTable);
            SetItemsSource();

            DataContext = CurrentTable;
        }

        private void SetParagraphSettingsEvents()
        {
            IParagraphSettings pSettings = ParagraphSettings as IParagraphSettings;

            pSettings.OnParagraphReplace = ReplaceParagraph;
            pSettings.OnParagraphShowOrHideHint = ShowOrHideHint;
            pSettings.OnParagraphDelete = DeleteParagraph;
        }

        private void SetItemsSource()
        {
            string[][] tableCells = CurrentTable.TableCells;

            for (int i = 0; i < tableCells[0].Length; i++)
            {
                DataGridTextColumn col = new DataGridTextColumn
                {
                    Binding = new Binding($"[{i}]")
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    }
                };

                TableDataGrid.Columns.Add(col);
            }
        }

        #region IParagraphEditView 
        public void DeleteParagraph()
        {
            ParentList.Remove(CurrentTable);
            RefreshParagraphsUI();
        }

        public void ReplaceParagraph()
        {
        }

        public void ShowOrHideHint()
        {
            if (HintControl.Visibility == Visibility.Collapsed)
            {
                HintControl.Visibility = Visibility.Visible;
            }
            else
            {
                HintControl.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        private void AddNewRightRow(object sender, RoutedEventArgs e)
        {
            DataGridCell clickedCell = e.OriginalSource as DataGridCell;
            DataGridColumn cellColumn = clickedCell.Column;

            int columnIndex = 0, rowIndex = 0;

            for (int i = 0; i<TableDataGrid.Columns.Count; i++)
                if (TableDataGrid.Columns[i].Equals(cellColumn))
                {
                    columnIndex = i;
                    break;
                }
        }

        private void AddNewLeftRow(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewUpCol(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewDownCol(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteCol(object sender, RoutedEventArgs e)
        {

        }
    }
}
