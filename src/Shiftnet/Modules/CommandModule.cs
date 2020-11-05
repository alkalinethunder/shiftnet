using System;
using AlkalineThunder.Pandemic;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;
using Shiftnet.Commands;

namespace Shiftnet.Modules
{
    public class CommandModule : EngineModule
    {
        private List<CommandInformation> _commands = new List<CommandInformation>();

        public IEnumerable<HelpEntry> AvailableCommands
            => _commands.Select(x => new HelpEntry(x.Name, x.Description));
        
        public async Task<bool> RunCommand(string name, string[] args, CancellationToken token, ConsoleControl console)
        {
            token.ThrowIfCancellationRequested();

            var command = _commands.FirstOrDefault(x => x.Name == name);
            if (command != null)
            {
                await command.Run(token, args, console);
                return true;
            }

            return false;
        }
        
        private void LocateCommands()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in asm.GetTypes().Where(x => x.InheritsFrom(typeof(Command)))
                        .Where(x => x.GetConstructor(Type.EmptyTypes) != null))
                    {
                        var attribute = type.GetCustomAttributes(false).OfType<CommandAttribute>().FirstOrDefault();

                        if (attribute != null)
                        {
                            var info = new ConsoleCommand(attribute.Name, attribute.Description, type);
                            _commands.Add(info);
                        }
                    }
                }
                catch
                {
                    
                }
            }
        }
        
        protected override void OnInitialize()
        {
            LocateCommands();
        }
    }
}