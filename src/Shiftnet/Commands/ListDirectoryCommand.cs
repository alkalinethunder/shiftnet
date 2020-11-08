using System.Threading.Tasks;

namespace Shiftnet.Commands
{
    [Command("ls", "List the files and folders in the current directory.")]
    public class ListDirectoryCommand : Command
    {
        protected override async Task Main()
        {
            foreach (var dir in ShiftOS.FileSystem.GetDirectories(CurrentDirectory))
                await this.WriteLine(dir);
            
            foreach (var dir in ShiftOS.FileSystem.GetFiles(CurrentDirectory))
                await this.WriteLine(dir);
        }
    }
}