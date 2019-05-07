using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLib.Word.Interfaces
{
    public interface IMainCommandsContainer : IWordMainCommand
    {
        IList<IWordMainCommand> MainCommands { get; }

        void Add(IWordMainCommand command);
        void Remove(IWordMainCommand command);
        void Refresh();
    }
}
