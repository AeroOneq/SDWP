using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ApplicationLib.Models;


namespace SDWP.Models
{
    public class TemplateTreeViewParagraphItem : TemplateTreeViewItem
    {
        private ImageSource imageSource;

        public Paragraph Paragraph { get; set; }
        public ImageSource ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public TemplateTreeViewParagraphItem(Paragraph paragraph) : base()
        {
            Paragraph = paragraph;
            HeaderText = Paragraph.ParagraphElement.Title;

            switch (paragraph.Type)
            {
                case "Subparagraph":
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/templateTreeViewSubparagraphIcon.png"));
                    break;

                case "Table":
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/templateTreeViewTableIcon.png"));
                    break;

                case "NumberedList":
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/templateTreeViewNumberedListIcon.png"));
                    break;

                case "ParagraphImage":
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/templateTreeViewImageIcon.png"));
                    break;
            }

            DataContext = this;
        }

        public override void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (prop == "HeaderText")
                Paragraph.ParagraphElement.Title = HeaderText;

            base.OnPropertyChanged(prop);
        }
    }
}
