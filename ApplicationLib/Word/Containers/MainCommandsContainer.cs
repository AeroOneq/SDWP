using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Containers
{
    class MainCommandsContainer : IMainCommandsContainer
    {
        public IList<IWordMainCommand> MainCommands { get; private set; } = new List<IWordMainCommand>();

        public void Add(IWordMainCommand command)
        {
            MainCommands.Add(command);
        }

        public void Remove(IWordMainCommand command)
        {
            MainCommands.Remove(command);
        }
        public void Refresh()
        {
            MainCommands = new List<IWordMainCommand>();
        }

        public void Render()
        {
            foreach (IWordMainCommand command in MainCommands)
            {
                command.Render();
            }
        }
    }
}
