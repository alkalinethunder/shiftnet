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
using Shiftnet.Modules;

namespace Shiftnet
{
    public class MainMenu : Scene
    {
        private Button _play;
        private Button _settings;
        private Button _exit;
        private Button _continue;
        private SettingsWindow _settingsWindow;

        private GameplayManager GameplayManager
            => GetModule<GameplayManager>();
        
        protected override void OnLoad()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/main.gui"));

            _play = Gui.FindById<Button>("new");
            _settings = Gui.FindById<Button>("settings");
            _continue = Gui.FindById<Button>("continue");
            _exit = Gui.FindById<Button>("exit");

            _play.Click += PlayOnClick;
            _settings.Click += SettingsOnClick;
            _exit.Click += ExitOnClick;
            _continue.Click += ContinueOnClick;
            
            base.OnLoad();
        }

        private void ContinueOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Close();
                _settingsWindow = null;
            }

            GameplayManager.ContinueGame();
        }

        private void PlayOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Close();
                _settingsWindow = null;
            }

            this.AskForText("New Game", "Please enter your new ShiftOS computer's name:", NewGameCallback,
                InfoboxValidators.NonWhite, InfoboxValidators.NoSpaces);
        }

        private void NewGameCallback(InfoboxResult result, string text)
        {
            if (result == InfoboxResult.OK)
            {
                GameplayManager.StartNewGame(text);
            }
        }

        private void SettingsOnClick(object? sender, MouseButtonEventArgs e)
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

        private void ExitOnClick(object? sender, MouseButtonEventArgs e)
        {
            this.App.Exit();
        }
    }
}
