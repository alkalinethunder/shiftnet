using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Theming;

namespace Shiftnet
{
    public class Desktop : Scene
    {
        // windows
        private SettingsWindow _settingsWindow;
        
        // gui elements
        private TextBlock _time = null;
        private Button _appLauncherOpen = null;
        private Button _settingsOpen = null;
        private ConsoleControl _console;
        private Process _proc;
        private Task _outTask;
        private Task _inTask;
        private object _inLock = new object();
        private object _outLock = new object();
            
        
        protected override void OnLoad()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/desktop.gui"));

            _time = Gui.FindById<TextBlock>("time");
            _appLauncherOpen = Gui.FindById<Button>("apps");
            _settingsOpen = Gui.FindById<Button>("settings");
            
            _settingsOpen.Click += SettingsOpenOnClick;

            _console = Gui.FindById<ConsoleControl>("console");
            StartConceptShell();
            
            base.OnLoad();
        }

        private void StartConceptShell()
        {
            _proc = new Process();
            _proc.StartInfo.FileName = "cmd.exe";
            _proc.StartInfo.RedirectStandardOutput = true;
            _proc.EnableRaisingEvents = true;
            _proc.StartInfo.RedirectStandardInput = true;
            _proc.Start();

            _outTask = ReadOutput();
            _inTask = SendInput();
        }
        
        private void SettingsOpenOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = OpenWindow<SettingsWindow>();
                _settingsWindow.Closed += (o, args) =>
                {
                    _settingsWindow = null;
                };
            }
            
            SceneSystem.SetFocus(_settingsWindow.Gui);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _time.Text = DateTime.Now.ToShortTimeString();

            base.OnUpdate(gameTime);
        }


        private async Task ReadOutput()
        {
            var block = new char[1024];
            
            while (true)
            {
                var bytesRead = await _proc.StandardOutput.ReadAsync(block, 0, block.Length);

                await App.GameLoop.Invoke(() =>
                {
                    for (var i = 0; i < bytesRead; i++)
                        _console.Write(block[i].ToString());
                });

            }
        }

        private async Task SendInput()
        {
            while (true)
            {
                var text = await _console.Input.ReadLineAsync();
                await App.GameLoop.Invoke(() =>
                {
                    _proc.StandardInput.WriteLine(text);
                });
            }
        }
    }
}
