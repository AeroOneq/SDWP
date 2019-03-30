using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

using ApplicationLib.Models;

namespace ApplicationLib.Views
{
    public partial class SubparagraphWatchView : UserControl
    {
        private readonly int maxLineSymbolsCount = 200;
        public Subparagraph Subparagraph { get; }
        private bool DoTextChangedActions { get; set; } = true;

        #region Constructors
        public SubparagraphWatchView() { InitializeComponent(); }

        public SubparagraphWatchView(Subparagraph subparagraph)
        {
            InitializeComponent();

            Subparagraph = subparagraph;
            subparagraphTextBlock.Text = subparagraph.Text;
        }
        #endregion

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
            string lastWord = text.Substring(lastSpaceIndex + 1);

            StringBuilder sb = new StringBuilder();
            sb.Append("\n").Append(lastWord);

            textBox.Text = text.Remove(lastSpaceIndex);
            textBox.Text += sb.ToString();
        }

        private void AppendNewLine(TextBox textBox) =>
            textBox.Text += "\n";
        #endregion

        private void SubparagraphKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int lastEnterSymbolIndex = textBox.Text.LastIndexOf("\n");

            if (e.Key == Key.Back && textBox.Text.Length > 0 &&
                textBox.Text[textBox.Text.Length - 1] == '\n' && textBox.LineCount > 1)
            {
                DeleteLastLine(textBox);
            }

            textBox.SelectionStart = textBox.Text.Length;
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
    }
}
