using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word.Interfaces
{
    /// <summary>
    /// Main command is a command which can be consisted of usual word commands.
    /// Main commands are used in IWordRenderer
    /// </summary>
    public interface IWordMainCommand
    {
        void Render();
    }
}
