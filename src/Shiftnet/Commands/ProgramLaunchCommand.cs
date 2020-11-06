using System;
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

        public override async Task Run(CancellationToken token, string[] args, ConsoleControl console, Desktop os)
        {
            await os.App.GameLoop.Invoke(() =>
            {
                _launcher.Launch(args, os);
            });
        }
    }
}