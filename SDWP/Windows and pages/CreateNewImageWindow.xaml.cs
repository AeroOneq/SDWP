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
using System.IO;

using ApplicationLib.Models;
using ApplicationLib.Interfaces;

using SDWP.Exceptions;
using SDWP.Factories;

using Microsoft.Win32;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для CreateNewImageWindow.xaml
    /// </summary>
    public partial class CreateNewImageWindow : Window
    {
        #region Properties
        private Item ContentItem { get; }

        private string ImagePathTextBoxHint { get; } = "Путь до рисунка...";
        private string ImageTitleTextBoxHint { get; } = "Введите название рисунка: ";

        private TextBox ImagePathTextBox { get; }
        private TextBox ImageTitleTextBox { get; }
        #endregion

        #region Services and factories
        private ISdwpAbstractFactory AbstractFactory { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        public CreateNewImageWindow(Item currentContentItem)
        {
            InitializeComponent();

            ContentItem = currentContentItem;

            ImagePathTextBox = imagePathTextBox;
            ImageTitleTextBox = imageTitleTextBox;

            InitializeServices();
        }

        private void InitializeServices()
        {
            AbstractFactory = new SdwpAbstractFactory();

            ExceptionHandler = AbstractFactory.GetExceptionHandler(Dispatcher);
        }

        #region Event handlers
        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ImagePathTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(ImagePathTextBoxHint, sender as TextBox);
        }

        private void ImagePathTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(ImagePathTextBoxHint, sender as TextBox);
        }

        private void ImageTitleTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxLostFocus(ImagePathTextBoxHint, sender as TextBox);
        }

        private void ImageTitleTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            OnTextBoxGotFocus(ImagePathTextBoxHint, sender as TextBox);
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

        public void SelectImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Выберете рисунок",
                CheckFileExists = true,
                Filter = "JPG files (*jpg)|*jpg|PNG files (*png)|*png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void CreateNewImage(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = ImagePathTextBox.Text;
                string imageTitle = ImageTitleTextBox.Text;
                byte[] imageByteArr = GetByteRepresentationOfImage(filePath);

                ParagraphImage paragraphImage = new ParagraphImage(imageByteArr)
                {
                    Title = imageTitle
                };
                (paragraphImage as IParentableParagraph).SetParents(ContentItem, ContentItem.Paragraphs);

                ContentItem.Paragraphs.Add(paragraphImage);

                DialogResult = true;
                Close();
            }
            catch (IOException ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleWithMessageBox(ex);
            }
        }

        private byte[] GetByteRepresentationOfImage(string filePath)
        {
            byte[] imageByteArr;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(0, SeekOrigin.Begin);
                imageByteArr = new byte[fs.Length];
                fs.Read(imageByteArr, 0, (int)fs.Length);
            }

            return imageByteArr;
        }
        #endregion
    }
}
