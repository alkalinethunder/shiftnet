using System;
using System.Threading;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Rendering;
using Microsoft.Xna.Framework;
using Plex.Objects;
using Shiftnet.Apps;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Windowing;

namespace Shiftnet
{
    public class MainMenu : Scene
    {
        private Button _play;
        private Button _settings;
        private Button _exit;
        
        protected override void OnLoad()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/main.gui"));

            _play = Gui.FindById<Button>("play");
            _settings = Gui.FindById<Button>("settings");
            _exit = Gui.FindById<Button>("exit");

            _settings.Click += SettingsOnClick;
            _exit.Click += ExitOnClick;
            
            base.OnLoad();
        }

        private void SettingsOnClick(object? sender, MouseButtonEventArgs e)
        {
            OpenWindow<SettingsWindow>();
        }

        private void ExitOnClick(object? sender, MouseButtonEventArgs e)
        {
            this.App.Exit();
        }
    }

    public class SettingsWindow : Window
    {
        
    }
}
