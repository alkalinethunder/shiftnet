using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet.Commands
{
    public abstract class Command
    {
        private CancellationToken _token;
        private string[] _args;
        private ConsoleControl _console;

        protected string[] Arguments => _args;

        protected string CurrentDirectory { get; private set; }
        
        protected IShiftOS ShiftOS { get; private set; }
        
        protected void ThrowIfCancelled()
        {
            _token.ThrowIfCancellationRequested();
        }

        protected async Task Write(string text)
        {
            ThrowIfCancelled();
            await _console.Output.WriteAsync(text);
            ThrowIfCancelled();
        }

        protected async Task WriteLine(string text)
        {
            await Write(text + Environment.NewLine);
        }

        protected async Task Write(string format, params object[] args)
        {
            var text = string.Format(format, args);
            await Write(text);
        }

        protected async Task WriteLine(string format, params object[] args)
        {
            await Write(format + Environment.NewLine, args);
        }

        protected async Task<string> ReadLine()
        {
            ThrowIfCancelled();

            var block = new char[1024];
            var line = "";
            var done = false;

            while (!done)
            {
                ThrowIfCancelled();
                var bytesRead = await _console.Input.ReadAsync(block, 0, block.Length);
                for (var i = 0; i < bytesRead; i++)
                {
                    var c = block[i];
                    if (c == '\n')
                    {
                        done = true;
                        break;
                    }

                    line += c;
                }
            }
            
            return line;
        }

        public async Task Run(CancellationToken token, string[] args, ConsoleControl console, IShiftOS shiftos, string cwd)
        {
            _args = args;
            _console = console;
            _token = token;

            ShiftOS = shiftos;
            
            CurrentDirectory = cwd;
            
            ThrowIfCancelled();
            await Main();
        }

        protected abstract Task Main();
    }
}