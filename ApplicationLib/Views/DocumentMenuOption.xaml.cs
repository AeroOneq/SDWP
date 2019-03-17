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
        private Document Document { get; }
        public Button DocumentBtn { get; } 

        public DocumentMenuOption() { InitializeComponent(); }
        public DocumentMenuOption(Document document)
        {
            InitializeComponent();
            Document = document;

            DocumentBtn = documentBtn;
            documentBtn.Content = Document.Name;
        }
    }
}
