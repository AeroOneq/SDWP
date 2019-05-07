﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Word.Interfaces;
using DocumentFormat.OpenXml.Packaging;

namespace ApplicationLib.Word.Containers
{
    class CommandsContainer : ICommandsContainer
    {
        public IList<IWordCommand> Commands { get; set; } = new List<IWordCommand>();

        public void Add(IWordCommand command)
        {
            Commands.Add(command);
        }

        public void Refresh()
        {
            Commands = new List<IWordCommand>();
        }

        public void Remove(IWordCommand command)
        {
            Commands.Remove(command);
        }

        public void Render()
        {
            foreach (IWordCommand command in Commands)
            {
                command.Render();
            }
        }
    }
}
