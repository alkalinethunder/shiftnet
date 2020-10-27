#define DEVEL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plex.Engine;
using Plex.Objects;

namespace Shiftnet
{
    public static class TerminalCommands
    {

        [MetaCommand]
        [Command("clear", description = "{DESC_CLEAR}")]
        public static bool Clear(ConsoleContext console)
        {
            console.Clear();

            return true;
        }
    }

    public static class PlexCommands
    {
        [Command("shutdown", description = "{DESC_SHUTDOWN}")]
        public static void Shutdown(ConsoleContext console)
        {
            ServerManager.Disconnect(DisconnectType.UserRequested);
        }
    }

    public static class WindowCommands
    {
        [Command("processes", description = "{DESC_PROCESSES}")]
        public static bool List()
        {
            Console.WriteLine("{GEN_CURRENTPROCESSES}");
            foreach (var app in AppearanceManager.OpenForms)
            {
                //Windows are displayed the order in which they were opened.
                Console.WriteLine($"{AppearanceManager.OpenForms.IndexOf(app)}\t{app.Text}");
            }
            return true;
        }

        [Command("programs", description = "{DESC_PROGRAMS}")]
        public static bool Programs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{GEN_PROGRAMS}");
            sb.AppendLine("===============");
            sb.AppendLine();
            //print all unique namespaces.
            foreach(var n in TerminalBackend.Commands.Where(x => x is TerminalBackend.WinOpenCommand && Upgrades.UpgradeInstalled(x.Dependencies)).OrderBy(x => x.CommandInfo.name))
            {
                sb.Append(" - " + n.CommandInfo.name);
                if (!string.IsNullOrWhiteSpace(n.CommandInfo.description))
                    if (Upgrades.UpgradeInstalled("help_description"))
                        sb.Append(" - " + n.CommandInfo.description);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());

            return true;
        }

        [Command("close", description ="{DESC_CLOSE}")]
        [UsageString("<pid>")]
        public static void CloseWindow(Dictionary<string, object> args)
        {
            int winNum = -1;

            if(!int.TryParse(args["<pid>"].ToString(), out winNum))
            {
                Console.WriteLine("Fatal error: process id must be a 32-bit integer");
                return;
            }

            string err = null;

            if (winNum < 0 || winNum >= AppearanceManager.OpenForms.Count)
                err = "Error: The window couldn't be found.";

            if (string.IsNullOrEmpty(err))
            {
                Console.WriteLine("Window closed.");
                AppearanceManager.Close(AppearanceManager.OpenForms[winNum].ParentWindow);
            }
            else
            {
                Console.WriteLine(err);
            }
        }

        
    }
}
