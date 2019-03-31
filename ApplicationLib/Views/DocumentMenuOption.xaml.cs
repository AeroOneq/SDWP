using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ApplicationLib.Models;

namespace ApplicationLib.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentMenuOption.xaml
    /// </summary>
    public partial class DocumentMenuOption : UserControl
    {
        #region Properties
        public Document Document { get; }
        public List<Document> ParentList { get; }

        public Button DocumentBtn { get; }
        private TextBox DocumentBtnTextBox { get; set; }
        #endregion

        #region Events 
        public EventHandler OnDocumentItemClick { get; set; }
        public Action UpdateList { get; set; }
        #endregion

        #region Constructors
        public DocumentMenuOption() { InitializeComponent(); }
        public DocumentMenuOption(Document document, List<Document> parentList)
        {
            InitializeComponent();

            Document = document;
            ParentList = parentList;

            DocumentBtn = documentBtn;
            DocumentBtnTextBox = DocumentBtn.Content as TextBox;
            DocumentBtnTextBox.Text = Document.Name;
            DocumentBtnTextBox.ContextMenu = ContextMenu;
        }
        #endregion

        private void OnDocumentBtnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Document.Name = DocumentBtnTextBox.Text;
                MakeDocumentBtnTextBoxReadOnly(DocumentBtnTextBox);
            }
        }

        private void MakeDocumentBtnTextBoxReadOnly(TextBox textBox)
        {
            textBox.IsReadOnly = true;
            textBox.Cursor = Cursors.Arrow;
        }

        private void MakeDocumentBtnTextBoxNotReadOnly(TextBox textBox)
        {
            textBox.IsReadOnly = false;
            textBox.Cursor = Cursors.IBeam;
        }

        private void OnDocumentBtnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            DocumentBtnTextBox.Text = Document.Name;
            MakeDocumentBtnTextBoxReadOnly(DocumentBtnTextBox);
        }

        private void OnDocumentBtnTextBoxClick(object sender, MouseButtonEventArgs e)
        {
            if (DocumentBtnTextBox.IsReadOnly)
            {
                OnDocumentItemClick(this, e);
            }
        }

        private void OnDocumentBtnTextBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartRenamingDocument(null, null);
        }

        private void StartRenamingDocument(object sender, RoutedEventArgs e)
        {
            MakeDocumentBtnTextBoxNotReadOnly(DocumentBtnTextBox);
        }

        private void DeleteDocument(object sender, RoutedEventArgs e)
        {
            ParentList.Remove(Document);
            UpdateList();
        }
    }
}
