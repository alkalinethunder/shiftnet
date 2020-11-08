using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;
using Shiftnet.Modules;

namespace Shiftnet.Commands
{
    public class ProgramLaunchCommand : CommandInformation
    {
        private AppLauncher _launcher;
        
        public ProgramLaunchCommand(AppLauncher launcher) : base(launcher.Command, launcher.Description)
        {
            _launcher = launcher;
        }

        public override async Task Run(CancellationToken token, string[] args, ConsoleControl console, Desktop os, string cwd)
        {
            await os.App.GameLoop.Invoke(() =>
            {
                try
                {
                    _launcher.Launch(args, os, cwd);
                }
                catch (Exception ex)
                {
                    console.WriteLine($"{_launcher.Name} was unable to launch.");
#if DEBUG
                    console.WriteLine(ex.ToString());
#endif
                }
            });
        }
    }
}