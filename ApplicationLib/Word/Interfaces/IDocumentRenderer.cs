using ApplicationLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word.Interfaces
{
    public interface IDocumentRenderer
    { 
        Task Render();
        void SetRenderParams(RenderSettings renderSettings,  Document document,
            Documentation documentation);
    }
}
