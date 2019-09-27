using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IImageSettings : IParagraphSettings
    {
        Action OnUploadNewImage { get; set; }
    }
}
