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
using ApplicationLib.Interfaces;

namespace ApplicationLib.Views
{
    public partial class NumberedListEditView : UserControl, IParagraphEditView
    {
        private NumberedList NumberedList { get; }
        private HintControl HintControl { get; }
        private ParagraphElementSettings ParagraphSettings { get; set; }

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        public List<IParagraphElement> ParentList { get; set; }
        #endregion


        #region Constructors
        public NumberedListEditView()
        {
            InitializeComponent();
        }
        public NumberedListEditView(NumberedList numberedList)
        {
            InitializeComponent();

            HintControl = hintControl;
            NumberedList = numberedList;
            ParagraphSettings = paragraphsSettings;

            DataContext = NumberedList;
            SetParagraphSettingsEvents();
        }
        #endregion

        private void SetParagraphSettingsEvents()
        {
            IParagraphSettings pSettings = ParagraphSettings as IParagraphSettings;

            pSettings.OnParagraphDelete += DeleteParagraph;
            pSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
            pSettings.OnParagraphReplace += ReplaceParagraph;
        }

        #region IParagraphEditView
        public void DeleteParagraph()
        {
            ParentList.Remove(NumberedList);
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
    }
}
