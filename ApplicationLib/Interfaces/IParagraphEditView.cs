using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IParagraphEditView
    {
        Action RefreshParagraphsUI { get; set; }
        List<IParagraphElement> ParentList { get; set; }

        void DeleteParagraph();
        void ReplaceParagraph();
        void ShowOrHideHint();
    }
}
