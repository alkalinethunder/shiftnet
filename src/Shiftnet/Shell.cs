using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;
using Shiftnet.Modules;
using System.Collections.Generic;
using System.Linq;

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
        private bool _running = false;

        private IEnumerable<string> Tips
        {
            get
            {
                yield return "Type 'version' to see the current ShiftOS version and kernel build.";
                yield return "Type 'clear' to clear the terminal.";
                yield return
                    "Type 'tips' to see these usage tips, 'commands' for a list of terminal commands, and 'programs' to see what you have installed.";
                yield return "Type 'help' for a complete help guide.";
                yield return "Press <Tab> to complete commands and file paths.";
                yield return
                    "Press <F10> to access Settings, <F11> to toggle Fullscreen Mode, and <F12> to toggle Dark Mode.";
                yield return "Use <Ctrl>+ and <Ctrl>- to zoom in and out of the terminal.";
            }
        }
        
        private CommandModule CommandModule
            => _os.GetModule<CommandModule>();

        private AppsModule Apps
            => _os.GetModule<AppsModule>();

        private CommandModule Commands
            => _os.GetModule<CommandModule>();
        
        public bool IsLoginShell { get; }
        
        public Shell(Desktop os, ConsoleControl console, bool isLogin)
        {
            IsLoginShell = isLogin;
            _os = os;
            _console = console;

            _stdout = _console.Output;
            _stdin = _console.Input;
        }

        private string[] Tokenize(string command)
        {
            var tokens = new List<string>();
            var inQuote = false;
            var escaping = false;
            var escapeChar = '\\';
            var quoteChar = '"';
            var token = "";

            foreach (var c in command)
            {
                if (escaping)
                {
                    token += c;
                    escaping = false;
                    continue;
                }

                if (c == escapeChar)
                {
                    escaping = true;
                    continue;
                }

                if (c == quoteChar)
                {
                    if (inQuote)
                    {
                        inQuote = false;
                        continue;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            inQuote = true;
                            continue;
                        }

                        throw new InvalidOperationException("unexpected start of string.");
                    }
                }

                if (char.IsWhiteSpace(c))
                {
                    if (!inQuote)
                    {
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            tokens.Add(token);
                            token = "";
                            continue;
                        }
                    }
                }

                token += c;
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                tokens.Add(token);
            }

            if (escaping)
                throw new InvalidOperationException("unfinished escape sequence.");
            if (inQuote)
                throw new InvalidOperationException("unended string.");

            return tokens.ToArray();
        }
        
        public void Start()
        {
            if (_shell == null)
            {
                _ct = new CancellationTokenSource();
                
                try
                {
                    _running = true;
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
                    _running = false;
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

        private async Task<bool> ProcessBuiltIn(string name, string[] args)
        {
            switch (name)
            {
                case "commands":
                    await _stdout.WriteLineAsync("TERMINAL COMMANDS");
                    await _stdout.WriteLineAsync("=================");
                    await _stdout.WriteLineAsync();

                    foreach (var command in Commands.AvailableCommands)
                    {
                        await _stdout.WriteLineAsync($" - {command.Name}: {command.Description}");
                    }
                    
                    await _stdout.WriteLineAsync();

                    return true;
                case "version":
                    await _stdout.WriteLineAsync("Welcome to ShiftOS - Version 30.04 (Linux kernel version 9.1.7420)");
                    await _stdout.WriteLineAsync();
                    return true;
                case "tips":
                    await _stdout.WriteLineAsync("USAGE TIPS:");
                    await _stdout.WriteLineAsync("===========");
                    await _stdout.WriteLineAsync();

                    foreach (var tip in Tips)
                    {
                        await _stdout.WriteLineAsync(" - " + tip);
                    }

                    await _stdout.WriteLineAsync();
                    
                    return true;
                case "programs":
                    await _stdout.WriteLineAsync("AVAILABLE PROGRAMS:");
                    await _stdout.WriteLineAsync("===================");
                    await _stdout.WriteLineAsync();

                    
                    foreach (var app in Apps.AvailableApps)
                    {
                        await _stdout.WriteLineAsync($" - {app.Name}: {app.Description}");
                    }

                    await _stdout.WriteLineAsync();

                    return true;
                case "help":
                    await ProcessBuiltIn("version", Array.Empty<string>());
                    await ProcessBuiltIn("tips", Array.Empty<string>());
                    await ProcessBuiltIn("commands", Array.Empty<string>());
                    await ProcessBuiltIn("programs", Array.Empty<string>());

                    return true;
                case "exit":
                    if (IsLoginShell)
                    {
                        await _stdout.WriteLineAsync("sh: exit: cannot exit login shell");
                    }
                    else
                    {
                        _running = false;
                    }

                    return true;
                case "shutdown":
                    if (IsLoginShell)
                    {
                        _running = false;
                        await _os.App.GameLoop.Invoke(() =>
                        {
                            _os.ShutDown();
                        });
                    }

                    return false;
            }

            return false;
        }
        
        private async Task ShellTask()
        {
            while (_running)
            {
                _token.ThrowIfCancellationRequested();
                await _stdout.WriteAsync("test> ");
                var text = await CancellableReadLine();

                var tokens = Array.Empty<string>();

                try
                {
                    tokens = Tokenize(text);
                }
                catch (InvalidOperationException ex)
                {
                    await _stdout.WriteLineAsync($"sh: error: {ex.Message}");
                    continue;
                }

                if (tokens.Any())
                {
                    var name = tokens.First();
                    var args = tokens.Skip(1).ToArray();

                    if (!await ProcessBuiltIn(name, args))
                    {
                        if (!await CommandModule.RunCommand(name, args, _token, _console, _os))
                        {
                            await _stdout.WriteLineAsync("Command not found.");
                        }
                    }
                }
            }
        }
    }

}