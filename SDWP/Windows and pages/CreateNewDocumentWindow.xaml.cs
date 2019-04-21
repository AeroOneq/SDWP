﻿using System;
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
using ApplicationLib.Services;
using ApplicationLib.Interfaces;

using SDWP.Factories;
using SDWP.Interfaces;
using SDWP.Exceptions;
using System.IO;

namespace SDWP
{
    /// <summary>
    /// Логика взаимодействия для CreateNewDocumentWindow.xaml
    /// </summary>
    public partial class CreateNewDocumentWindow : Window
    {
        #region Properties
        private List<Document> Documents { get; }
        private Documentation Documentation { get; }

        private List<LocalTemplate> LocalTemplates { get; set; }
        private string DefaultStoragePath { get; } = @"C:\Users\Aero\Desktop\Templates";

        private TextBlock LocalTemplatesTextBlock { get; set; }
        private TextBlock CloudTemplatesTextBlock { get; set; }
        private ComboBox LocalTemplatesComboBox { get; set; }
        private ComboBox CloudTemplateComboBox { get; set; }
        private TextBox DocumentNameTextBox { get; set; }
        #endregion

        #region Services and factories
        private IServiceAbstractFactory ServiceAbstractFactory { get; set; }
        private ISdwpAbstractFactory SdwpAbstractFactory { get; set; }

        private ILocalTemplateService LocalTemplateService { get; set; }
        private IExceptionHandler ExceptionHandler { get; set; }
        #endregion

        public CreateNewDocumentWindow(List<Document> documents, Documentation documentation)
        {
            InitializeComponent();
            InitializeServices();
            InitializeProperties();

            Documentation = documentation;
            Documents = documents;
        }

        private void InitializeProperties()
        {
            LocalTemplatesTextBlock = offlineTemplatesTextBlock;
            CloudTemplatesTextBlock = onlineTemplatesTextBlock;
            LocalTemplatesComboBox = localTemplatesComboBox;
            CloudTemplateComboBox = cloudTemplatesComboBox;
            DocumentNameTextBox = documentNameTextBox;
        }

        private void InitializeServices()
        {
            ServiceAbstractFactory = new ServiceAbstractFactory();
            SdwpAbstractFactory = new SdwpAbstractFactory();

            LocalTemplateService = ServiceAbstractFactory.GetLocalTemplateService();
            ExceptionHandler = SdwpAbstractFactory.GetExceptionHandler(Dispatcher);
        }

        private async Task GetLocalTemplates()
        {
            try
            {
                LocalTemplateService.StoragePath = DefaultStoragePath;
                LocalTemplates = (await LocalTemplateService.GetLocalTemplates()).ToList();
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

        #region Event handlers
        private void CancelCreation(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Creates a new document with items from a selected template
        /// </summary>
        private void CreateNewDocument(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DocumentNameTextBox.Text) || string.IsNullOrWhiteSpace(DocumentNameTextBox.Text))
            {
                SDWPMessageBox.ShowSDWPMessageBox("Ошибка", "Введите имя документа", MessageBoxButton.OK);
                return;
            }

            Document document = GetDocumentObject();
            if (LocalTemplatesComboBox.Visibility == Visibility.Visible)
            {
                LocalTemplate selectedTemplate = LocalTemplatesComboBox.SelectedItem as LocalTemplate;
                document.Items = selectedTemplate.Template.Items;

                Documents.Add(document);
            }
            else
            {
#warning create logic for creating doc from cloud template
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Creates and initializes the document object with necessary values
        /// </summary>
        private Document GetDocumentObject()
        {
            return new Document()
            {
                Name = DocumentNameTextBox.Text,
                CreationDate = DateTime.Now,
                AuthorID = UserInfo.CurrentUser.ID,
                UpdatedAt = DateTime.Now,
                AuthorName = UserInfo.CurrentUser.Name,
                Access = Access.Public,
                DocumentationID = Documentation.ID,
            };
        }

        private async void LocalTemplatesTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            LocalTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
            CloudTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.Black);

            await GetLocalTemplates();
            AddEmptyTemplateToLocalTemplates();

            LocalTemplatesComboBox.ItemsSource = null;
            LocalTemplatesComboBox.ItemsSource = LocalTemplates;

            if (LocalTemplates.Count > 0)
                LocalTemplatesComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds an local template with an empty Items so that user can create an empty document
        /// </summary>
        private void AddEmptyTemplateToLocalTemplates()
        {
            Template emptyTemplate = new Template()
            {
                Items = new List<Item>()
            };
            LocalTemplate emptyLocalTemplate = new LocalTemplate(emptyTemplate)
            {
                FileName = "Пустой документ"
            };

            List<LocalTemplate> localTemplates = new List<LocalTemplate>
            {
                emptyLocalTemplate
            };
            localTemplates.AddRange(LocalTemplates);

            LocalTemplates = localTemplates;
        }

        private void CloudTemplatesTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            LocalTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.Black);
            CloudTemplatesTextBlock.Foreground = new SolidColorBrush(Colors.OrangeRed);
        }

        private void TemplateTypesTextBlockMouseEnter(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Add(TextDecorations.Underline);
        }

        private void TemplateTypesTextBlockMouseLeave(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).TextDecorations.Clear();
        }
        #endregion
    }
}