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
using System.IO;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для CreateTemplateTreeViewParagraphWindow.xaml
    /// </summary>
    public partial class CreateTemplateTreeViewParagraphWindow : Window
    {
        private Item CurrentItem { get; set; }

        #region Constructors
        public CreateTemplateTreeViewParagraphWindow() { InitializeComponent(); }
        public CreateTemplateTreeViewParagraphWindow(Item currentItem)
        {
            InitializeComponent();

            CurrentItem = currentItem;
        }
        #endregion

        private void CreateNewParagraph(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(paragraphNameTextBox.Text))
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Введите имя нового параграфа",
                    MessageBoxButton.OK);
                return;
            }

            if (tableCreationModeRadioBtn.IsChecked == true)
            {
                Paragraph paragraph = new Paragraph()
                {
                    ParagraphElement = new Table()
                    {
                        Title = paragraphNameTextBox.Text,
                        TableCells = new string[2][]
                        {
                            new string[2] {string.Empty, string.Empty},
                            new string[2] {string.Empty, string.Empty},
                        }
                    },
                    Type = "Table"
                };

                (paragraph as IParentableParagraph).SetParents(CurrentItem, CurrentItem.Paragraphs);
                CurrentItem.Paragraphs.Add(paragraph);
            }
            else if (numberedListCreationModeRadioBtn.IsChecked == true)
            {
                Paragraph paragraph = new Paragraph()
                {
                    ParagraphElement = new NumberedList()
                    {
                        Title = paragraphNameTextBox.Text,
                        ListElements = new List<NumberedListElement>()
                        {
                            new NumberedListElement()
                            {
                                Index = "0",
                            }
                        }
                    },
                    Type = "NumberedList"
                };

                (paragraph as IParentableParagraph).SetParents(CurrentItem, CurrentItem.Paragraphs);
                CurrentItem.Paragraphs.Add(paragraph);
            }
            else if (imageCreationModeRadioBtn.IsChecked == true)
            {
                Paragraph paragraph = new Paragraph()
                {
                    ParagraphElement = new ParagraphImage()
                    {
                        Title = paragraphNameTextBox.Text,
                        ImageSource = GetResourceImageBytes("pack://application:,,,/Resources/defaultParagraphImageImage.png")
                    },
                    Type = "ParagraphImage"
                };

                (paragraph as IParentableParagraph).SetParents(CurrentItem, CurrentItem.Paragraphs);
                CurrentItem.Paragraphs.Add(paragraph);
            }
            else if (subparagraphCreationModeRadioBtn.IsChecked == true)
            {
                Paragraph paragraph = new Paragraph()
                {
                    ParagraphElement = new Subparagraph()
                    {
                        Title = paragraphNameTextBox.Text,
                        Text = string.Empty
                    },
                    Type = "Subparagraph"
                };

                (paragraph as IParentableParagraph).SetParents(CurrentItem, CurrentItem.Paragraphs);
                CurrentItem.Paragraphs.Add(paragraph);
            }

            DialogResult = true;
            Close();
        }

        private byte[] GetResourceImageBytes(string uri)
        {
            byte[] imageBytes;
            Uri imageUri = new Uri(uri);
            BitmapImage bitmapImage = new BitmapImage(imageUri);

            using (var ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                imageBytes = ms.ToArray();
            }

            return imageBytes;
        }

        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
