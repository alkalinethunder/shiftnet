using System;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet.Commands
{
    public class ConsoleCommand : CommandInformation
    {
        private Type _type;

        public ConsoleCommand(string name, string desc, Type type) : base(name, desc)
        {
            _type = type;
        }
        
        public override async Task Run(CancellationToken token, string[] args, ConsoleControl console, Desktop os)
        {
            token.ThrowIfCancellationRequested();

            var cmd = (Command) Activator.CreateInstance(_type, null);

            await cmd.Run(token, args, console);
        }

    }
}