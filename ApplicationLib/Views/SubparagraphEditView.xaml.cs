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
using System.Windows.Media.Animation;

using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Views
{
    public partial class SubparagraphEditView : UserControl, IParagraphEditView
    {
        public Subparagraph Subparagraph { get; }

        private readonly int maxLineSymbolsCount = 20;
        private bool DoTextChangedActions { get; set; } = true;

        private ParagraphElementSettings ParagraphSettings { get; }
        private HintControl HintControl { get; set; }

        #region IParagraphEditView properties
        public Action RefreshParagraphsUI { get; set; }
        #endregion

        #region Constructors
        public SubparagraphEditView() { InitializeComponent(); }

        public SubparagraphEditView(Subparagraph subparagraph)
        {
            InitializeComponent();

            Subparagraph = subparagraph;

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
            pSettings.OnParagraphReplace += ReplaceParagraph;
        }

        private void SubparagraphTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;

            if (DoWeNeedToCreateNewLine(text))
            {
                int lastSpaceIndex = text.LastIndexOf(" ");

                if (lastSpaceIndex > -1 && lastSpaceIndex > text.LastIndexOf("\n") + 1)
                    AppendNewLineWithLastWord(textBox, text, lastSpaceIndex);
                else if (text.Length != 0)
                    AppendNewLine(textBox);

                textBox.SelectionStart = textBox.Text.Length;
            }

            if (!DoTextChangedActions)
                DoTextChangedActions = true;
        }

        #region Utility methods for Text Changed event
        private bool DoWeNeedToCreateNewLine(string text)
        {
            return DoTextChangedActions && (text.LastIndexOf("\n") > -1 &&
                text.Substring(text.LastIndexOf("\n") + 1).Length == maxLineSymbolsCount) ||
                text.Length == maxLineSymbolsCount;
        }

        private void AppendNewLineWithLastWord(TextBox textBox, string text, int lastSpaceIndex)
        {
            DoTextChangedActions = false;
            string lastWord = text.Substring(lastSpaceIndex + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("\n").Append(lastWord);

            string newText = text.Remove(lastSpaceIndex);
            newText += sb.ToString();
            textBox.Text = newText;
        }

        private void AppendNewLine(TextBox textBox)
        {
            if (DoTextChangedActions)
                textBox.Text += "\n";
        }
        #endregion

        private void SubparagraphKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int lastEnterSymbolIndex = textBox.Text.LastIndexOf("\n");

            if (e.Key == Key.Back && textBox.Text.Length > 0 &&
                textBox.Text[textBox.Text.Length - 1] == '\n' && textBox.LineCount > 1)
            {
                DeleteLastLine(textBox);
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        #region PreviewKeyDown utility methods
        private void DeleteLastLine(TextBox textBox)
        {
            string text = textBox.Text;
            string lastText = text.Substring(text.Length - 2, 1);
            text = text.Remove(text.Length - 1);

            if (!string.IsNullOrEmpty(lastText) && lastText.Length > 0 &&
                lastText.IndexOf("\n") == -1)
                text += lastText;

            textBox.Text = text;
            DoTextChangedActions = false;
        }
        #endregion

        #region IParagraphEditView methods
        public void DeleteParagraph()
        {
            (Subparagraph as IParagraphElement).RemoveParagraphFromParentList();
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
