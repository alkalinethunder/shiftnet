using System.Threading.Tasks;

namespace Shiftnet.Commands
{
    [Command("echo", "Write text to the console.")]
    public class EchoCommand : Command
    {
        protected override async Task Main()
        {
            var argString = string.Join(" ", Arguments);
            await WriteLine(argString);
        }
    }
}