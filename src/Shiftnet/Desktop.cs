﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        
        protected override void OnLoad()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/desktop.gui"));

            _time = Gui.FindById<TextBlock>("time");
            _appLauncherOpen = Gui.FindById<Button>("apps");
            _settingsOpen = Gui.FindById<Button>("settings");
            
            _settingsOpen.Click += SettingsOpenOnClick;
            
            base.OnLoad();
        }

        private void SettingsOpenOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = OpenWindow<SettingsWindow>();
            }
            
            SceneSystem.SetFocus(_settingsWindow.Gui);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _time.Text = DateTime.Now.ToShortTimeString();
            base.OnUpdate(gameTime);
        }
    
    }
}
