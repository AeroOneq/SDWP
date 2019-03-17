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
    /// Логика взаимодействия для SubparagraphView.xaml
    /// </summary>
    public partial class SubparagraphWatchView : UserControl
    {
        public Subparagraph Subparagraph { get; }

        #region Constructors
        public SubparagraphWatchView() { InitializeComponent(); }
        public SubparagraphWatchView(Subparagraph subparagraph)
        {
            InitializeComponent();

            Subparagraph = subparagraph;
            subparagraphTextBlock.Text = subparagraph.Text;
        }
        #endregion
    }
}
