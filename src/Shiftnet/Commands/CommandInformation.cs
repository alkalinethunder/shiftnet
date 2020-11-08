using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet.Commands
{
    public abstract class CommandInformation
    {
        public string Name { get; }
        public string Description { get; }

        public CommandInformation(string name, string desc)
        {
            Name = name;
            Description = desc;
        }

        public abstract Task Run(CancellationToken token, string[] args, ConsoleControl console, Desktop desktop, string cwd);
    }
}