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
using System.Windows.Shapes;

using ApplicationLib.Models;
using ApplicationLib.Views;

namespace ApplicationLib.Interfaces
{
    public interface IParagraphElement
    {
        string Hint { get; set; }
        string Title { get; set; }

        UserControl GetWatchView();
        UserControl GetEditView();
        void RemoveParagraphFromParentList();
    }
}
