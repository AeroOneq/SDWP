using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Models;
using ApplicationLib.Word;

namespace ApplicationLib.Interfaces
{
    public interface IWordRenderer : IDocumentRenderer
    {
        void SetRenderParams(RenderSettings renderSettings, Document document,
            Documentation documentation);
    }
}
