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
using System.Windows.Shapes;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

namespace SDWP
{
    public partial class CreateNewNumberedListWindow : Window
    {
        private string NumberedListTitleHintText { get; } = "Название списка...";
        private TextBox NumberedListTitleTextBox { get; }
        private Item ContentItem { get; }

        public CreateNewNumberedListWindow(Item contentItem)
        {
            InitializeComponent();

            NumberedListTitleTextBox = numberedListTitleTextBox;
            ContentItem = contentItem;
        }

        private void NumberedListTitleTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(NumberedListTitleHintText, sender as TextBox);
        }

        private void NumberedListTitleTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(NumberedListTitleHintText, sender as TextBox);
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

        private void CreateNewNumberedList(object sender, RoutedEventArgs e)
        {
            string listTitle = NumberedListTitleTextBox.Text;
            if (!string.IsNullOrEmpty(listTitle) && !(listTitle == NumberedListTitleHintText))
            {
                NumberedList numberedList = new NumberedList(new List<NumberedListElement>()
                {
                    new NumberedListElement(string.Empty)
                })
                {
                    Title = listTitle,
                };

                Paragraph paragraph = new Paragraph(typeof(NumberedList).Name, numberedList);
                (paragraph as IParentableParagraph).SetParents(ContentItem, ContentItem.Paragraphs);

                ContentItem.Paragraphs.Add(paragraph);

                DialogResult = true;
                Close();
            }
        }
    }
}
