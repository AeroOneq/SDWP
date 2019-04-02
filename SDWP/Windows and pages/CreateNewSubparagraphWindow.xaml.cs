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

namespace SDWP
{
    public partial class CreateNewSubparagraphWindow : Window
    {
        #region Properties
        private string SubparagraphTitleTextBoxHint { get; } = "Имя абзаца...";
        private TextBox SubparagraphTitleTextBox { get; }

        private Item CurrentItem { get; }
        #endregion

        public CreateNewSubparagraphWindow(Item currentItem)
        {
            InitializeComponent();

            CurrentItem = currentItem;

            SubparagraphTitleTextBox = subparagraphTitleTextBox;
        }

        #region Event handlers
        private void SubparagraphTitleTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(SubparagraphTitleTextBoxHint, sender as TextBox);
        }

        private void SubparagraphTitleTextBoxGotFocu(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(SubparagraphTitleTextBoxHint, sender as TextBox);
        }

        private void OnTextBoxGotFocus(string textBoxHint, TextBox textBox)
        {
            if (textBox.Text == textBoxHint)
                textBox.Text = string.Empty;
        }

        private void OnTextBoxLostFocus(string textBoxHint, TextBox textBox)
        {
            if (textBox.Text == string.Empty)
                textBox.Text = textBoxHint;
        }

        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CreateNewSubparagraph(object sender, RoutedEventArgs e)
        {
            string subparagraphTitle = SubparagraphTitleTextBox.Text;
            if (CheckSubparagraphTitle(subparagraphTitle))
            {
                Subparagraph subparagraph = new Subparagraph(CurrentItem)
                {
                    Title = subparagraphTitle
                };

                CurrentItem.Paragraphs.Add(subparagraph);

                DialogResult = true;
                Close();
            }
            else
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка при создании абзаца", "Непрвильно введены данные",
                    MessageBoxButton.OK);
            }
        }

        private bool CheckSubparagraphTitle(string title)
        {
            return !string.IsNullOrEmpty(title) && !(title == SubparagraphTitleTextBoxHint);
        }
        #endregion
    }
}
