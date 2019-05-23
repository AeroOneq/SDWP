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
using System.Windows.Media.Animation;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Views
{
    public partial class SubparagraphEditView : UserControl, IParagraphEditView
    {
        private Subparagraph Subparagraph { get; }
        public Paragraph Paragraph { get; }

        private bool DoTextChangedActions { get; set; } = true;

        private ParagraphElementSettings ParagraphSettings { get; }
        private HintControl HintControl { get; set; }

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        public Action RefreshParagraphsUIAfterSwap { get; set; }
        #endregion

        #region Constructors
        public SubparagraphEditView() { InitializeComponent(); }

        public SubparagraphEditView(Paragraph paragraph)
        {
            InitializeComponent();

            Paragraph = paragraph;
            Subparagraph = Paragraph.ParagraphElement as Subparagraph;

            DataContext = Subparagraph;

            ParagraphSettings = paragraphsSettings;
            HintControl = hintControl;
            HintControl.SetBinding(Subparagraph);

            SetParagraphSettingsEvents();
        }
        #endregion


        private void SetParagraphSettingsEvents()
        {
            IParagraphSettings pSettings = ParagraphSettings as IParagraphSettings;

            pSettings.OnParagraphDelete += DeleteParagraph;
            pSettings.OnParagraphShowOrHideHint += ShowOrHideHint;
            pSettings.MoveParagraphDown = MoveParagraphDown;
            pSettings.MoveParagraphUp = MoveParagraphUp;
        }

        #region IParagraphEditView methods
        public void DeleteParagraph()
        {
            (Paragraph as IParentableParagraph).RemoveParagraphFromParentList();
            RefreshParagraphsUI();
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

        public void MoveParagraphUp()
        {
            if (Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph)) == 0)
            {
                Paragraph.ParentList.Remove(Paragraph);
                Paragraph.ParentList.Add(Paragraph);
            }
            else
            {
                int itemIndex = Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph));

                Paragraph temp = Paragraph.ParentList[itemIndex - 1];
                Paragraph.ParentList[itemIndex - 1] = Paragraph;
                Paragraph.ParentList[itemIndex] = temp;
            }

            RefreshParagraphsUIAfterSwap();
        }

        public void MoveParagraphDown()
        {
            if (Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph)) == Paragraph.ParentList.Count - 1)
            {
                for (int i = Paragraph.ParentList.Count - 1; i > 0; i--)
                {
                    Paragraph.ParentList[i] = Paragraph.ParentList[i - 1];
                }

                Paragraph.ParentList[0] = Paragraph;
            }
            else
            {
                int itemIndex = Paragraph.ParentList.FindIndex(i => i.Equals(Paragraph));

                Paragraph temp = Paragraph.ParentList[itemIndex + 1];
                Paragraph.ParentList[itemIndex + 1] = Paragraph;
                Paragraph.ParentList[itemIndex] = temp;
            }

            RefreshParagraphsUIAfterSwap();
        }
        #endregion
    }
}
