using System;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet.Commands
{
    public class ConsoleCommand : CommandInformation
    {
        private Type _type;
        private bool _playerOnly;

        public override bool PlayerOnly => _playerOnly;
        
        public ConsoleCommand(string name, string desc, Type type, bool playerOnly) : base(name, desc)
        {
            _type = type;
            _playerOnly = playerOnly;
        }
        
        public override async Task Run(CancellationToken token, string[] args, ConsoleControl console, Desktop os, string cwd)
        {
            token.ThrowIfCancellationRequested();

            var cmd = (Command) Activator.CreateInstance(_type, null);

            await cmd.Run(token, args, console, os.CurrentOS, cwd);
        }

    }
}