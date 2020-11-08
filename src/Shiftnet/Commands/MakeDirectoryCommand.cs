using System.Threading.Tasks;

namespace Shiftnet.Commands
{
    [Command("mkdir", "Create a folder.")]
    public class MakeDirectoryCommand : Command
    {
        protected override Task Main()
        {
            var path = string.Join(' ', Arguments);
            var absolute = path.StartsWith("/") ? path : Paths.Combine(CurrentDirectory, path);

            ShiftOS.FileSystem.CreateDirectory(absolute);
            
            return Task.CompletedTask;
        }
    }
}