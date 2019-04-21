using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IParagraphSettings
    {
        Action OnParagraphDelete { get; set; }
        Action OnParagraphShowOrHideHint { get; set; }
        Action OnParagraphReplace { get; set; }
    }
}
