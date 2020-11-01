using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet
{
    public class Shell
    {
        private ConsoleControl _console;
        private StreamReader _stdin;
        private StreamWriter _stdout;
        private Task _shell;
        private CancellationTokenSource _ct = null;
        private CancellationToken _token;
        private Desktop _os;

        public Shell(Desktop os, ConsoleControl console)
        {
            _os = os;
            _console = console;

            _stdout = _console.Output;
            _stdin = _console.Input;
        }

        public void Start()
        {
            if (_shell == null)
            {
                _ct = new CancellationTokenSource();
                
                try
                {
                    _token = _ct.Token;
                    _shell = ShellTask();
                }
                catch (OperationCanceledException)
                {
                    _shell = null;
                }
            }
        }

        public void Stop()
        {
            if (_shell != null)
            {
                try
                {
                    _ct.Cancel();
                    _shell.Wait(_token);
                }
                catch (OperationCanceledException)
                {
                    _ct.Dispose();
                }
                finally
                {
                    _shell = null;
                    _ct = null;
                }
            }
        }

        private async Task<string> CancellableReadLine()
        {
            var line = "";
            var block = new char[1024];
            var done = false;
            
            while (!done)
            {
                _token.ThrowIfCancellationRequested();
                var bytesRead = await _stdin.ReadAsync(block, 0, block.Length);
                _token.ThrowIfCancellationRequested();

                for (var i = 0; i < bytesRead; i++)
                {
                    char c = block[i];
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
        
        private async Task ShellTask()
        {
            while (true)
            {
                _token.ThrowIfCancellationRequested();
                await _stdout.WriteAsync("test> ");
                var text = await CancellableReadLine();
                await _stdout.WriteLineAsync(text);
            }
        }
    }
}