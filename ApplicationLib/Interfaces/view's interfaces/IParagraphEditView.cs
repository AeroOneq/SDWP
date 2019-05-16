using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IParagraphEditView
    {
#warning add this to the table
        Paragraph Paragraph { get; }
        Action RefreshParagraphsUI { get; set; }
#warning add this to the table
        Action RefreshParagraphsUIAfterSwap { get; set; }

        void DeleteParagraph();
        void ShowOrHideHint();
        void MoveParagraphUp();
        void MoveParagraphDown();
    }
}
