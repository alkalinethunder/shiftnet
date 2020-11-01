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
        private Button _leave;
        private ConsoleControl _console;
        private Shell _shell;    
        
        protected override void OnLoad()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/desktop.gui"));

            _time = Gui.FindById<TextBlock>("time");
            _appLauncherOpen = Gui.FindById<Button>("apps");
            _settingsOpen = Gui.FindById<Button>("settings");
            _leave = Gui.FindById<Button>("leave");
            
            _settingsOpen.Click += SettingsOpenOnClick;

            _leave.Click += LeaveOnClick;
            
            _console = Gui.FindById<ConsoleControl>("console");
            StartShell();
            
            base.OnLoad();
        }

        private void LeaveOnClick(object? sender, MouseButtonEventArgs e)
        {
            var infobox = OpenWindow<Infobox>();
            infobox.Message = "Are you sure you want to exit ShiftOS?";
            infobox.Title = "Shut down";
            infobox.Buttons = InfoboxButtons.YesNo;
            infobox.Dismissed += InfoboxOnDismissed;
        }

        private void InfoboxOnDismissed(object? sender, InfoboxDismissedEventArgs e)
        {
            if (e.Result == InfoboxResult.Yes)
            {
                SceneSystem.GoToScene<MainMenu>();
            }
        }

        private void StartShell()
        {
            _shell = new Shell(this, _console);
            _shell.Start();

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

        protected override void OnUnload()
        {
            _shell.Stop();
            base.OnUnload();
        }
    }
}
