using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Models;
using ApplicationLib.Word;

namespace ApplicationLib.Word.Interfaces
{
    public interface IWordRenderer : IDocumentRenderer
    {
        ICommandsContainer CommandsContainer { get; set; }
    }
}
