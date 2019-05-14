using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Views
{
    public partial class TableEditView : UserControl, IParagraphEditView
    {
        private Table CurrentTable { get; }
        private Paragraph Paragraph { get; }

        private DataGrid TableDataGrid { get; }
        private HintControl HintControl { get; }
        private ParagraphElementSettings ParagraphSettings { get; }

        #region IParagraphEditView
        public Action RefreshParagraphsUI { get; set; }
        #endregion

        public TableEditView(Paragraph paragraph)
        {
            InitializeComponent();

            Paragraph = paragraph;
            CurrentTable = Paragraph.ParagraphElement as Table;

            TableDataGrid = tableDataGrid;
            HintControl = hintControl;
            ParagraphSettings = paragraphsSettings;

            SetParagraphSettingsEvents();
            HintControl.SetBinding(CurrentTable);
            SetItemsSource();
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
            TableDataGrid.Columns.Clear();
            string[][] tableCells = CurrentTable.TableCells;

            if (tableCells != null && tableCells.Length > 0 && tableCells[0] != null)
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

            RefreshDataContext();
        }

        private void RefreshDataContext()
        {
            DataContext = null;
            DataContext = CurrentTable;
        }

        #region IParagraphEditView 
        public void DeleteParagraph()
        {
            (Paragraph as IParentableParagraph).RemoveParagraphFromParentList();
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

        public void AddNewRightCol(object sender, RoutedEventArgs e)
        {
            CurrentTable.AddNewRightCol(GetSelectedCellCol());
            SetItemsSource();
        }

        public void AddNewLeftCol(object sender, RoutedEventArgs e)
        {
            CurrentTable.AddNewLeftCol(GetSelectedCellCol());
            SetItemsSource();
        }

        public void AddNewUpRow(object sender, RoutedEventArgs e)
        {
            CurrentTable.AddNewUpRow(GetSelectedCellRow());
            SetItemsSource();
        }

        public void AddNewDownRow(object sender, RoutedEventArgs e)
        {
            CurrentTable.AddNewDownRow(GetSelectedCellRow());
            SetItemsSource();
        }

        public void DeleteRow(object sender, RoutedEventArgs e)
        {
            CurrentTable.DeleteRow(GetSelectedCellRow());
            SetItemsSource();
        }

        public void DeleteCol(object sender, RoutedEventArgs e)
        {
            CurrentTable.DeleteCol(GetSelectedCellCol());
            SetItemsSource();
        }

        public int GetSelectedCellRow()
        {
            DataGridCellInfo selectedCell = TableDataGrid.SelectedCells[0];
            for (int i = 0; i < CurrentTable.TableCells.Length; i++)
                if (CurrentTable.TableCells[i].Equals(selectedCell.Item as string[]))
                    return i;

            return 0;
        }

        public int GetSelectedCellCol()
        {
            DataGridColumn selectedCol = TableDataGrid.SelectedCells[0].Column;
            for (int i = 0; i < TableDataGrid.Columns.Count; i++)
                if (selectedCol.Equals(TableDataGrid.Columns[i]))
                    return i;

            return 0;
        }
    }
}
