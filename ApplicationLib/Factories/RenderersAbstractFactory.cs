using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Interfaces;
using ApplicationLib.Word;
using ApplicationLib.Models;
using ApplicationLib.Word.Interfaces;

namespace ApplicationLib.Factories
{
    public class RenderersAbstractFactory : IRenderersAbstractFactory
    {
        public IWordRenderer GetWordDocumentRender()
        {
            return new WordRenderer();
        }
    }
}
