using ApplicationLib.Models;
using ApplicationLib.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Interfaces
{
    public interface IRenderersAbstractFactory
    {
        IWordRenderer GetWordDocumentRender();
    }
}
