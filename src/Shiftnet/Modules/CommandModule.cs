using System;
using AlkalineThunder.Pandemic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;
using Shiftnet.Commands;

namespace Shiftnet.Modules
{
    [RequiresModule(typeof(AppsModule))]
    public class CommandModule : EngineModule
    {
        private List<CommandInformation> _commands = new List<CommandInformation>();

        public AppsModule AppsModule
            => GetModule<AppsModule>();
        
        public IEnumerable<HelpEntry> AvailableCommands
            => _commands.Select(x => new HelpEntry(x.Name, x.Description));
        
        public async Task<bool> RunCommand(string name, string[] args, CancellationToken token, ConsoleControl console, Desktop os, string cwd)
        {
            if (!os.CurrentOS.FileSystem.DirectoryExists(cwd))
                throw new IOException("Current working directory doesn't exist.");

            token.ThrowIfCancellationRequested();

            var command = _commands.FirstOrDefault(x => x.Name == name);
            if (command != null)
            {
                await command.Run(token, args, console, os, cwd);
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

        private void LoadAppCommands()
        {
            foreach (var app in AppsModule.AvailableAppLaunchers)
            {
                if (app.HasCommand)
                {
                    var command = new ProgramLaunchCommand(app);
                    _commands.Add(command);
                }
            }
        }
        
        protected override void OnInitialize()
        {
            LocateCommands();
            LoadAppCommands();
        }
    }
}