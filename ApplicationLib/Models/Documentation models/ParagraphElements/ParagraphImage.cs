using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ApplicationLib.Interfaces;

namespace ApplicationLib.Models
{
    public class ParagraphImage : IParagraphElement
    {
        public byte[] ImageSource { get; set; }
        public Paragraph ParentParagraph { get; }

        public ParagraphImage(byte[] imageSource, Paragraph parentParagraph)
        {
            ImageSource = imageSource;
            ParentParagraph = parentParagraph;
        }

        public FrameworkElement VisualizeElement()
        {
            throw new NotImplementedException();
        }

        public FrameworkElement GetWatchView()
        {
            throw new NotImplementedException();
        }

        public FrameworkElement GetEditView()
        {
            throw new NotImplementedException();
        }

        public Task DeleteParagraph()
        {
            throw new NotImplementedException();
        }
    }
}
