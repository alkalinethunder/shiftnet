using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Rendering;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Plex.Engine;
using Plex.Engine.GraphicsSubsystem;
using Plex.Engine.Theming;
using Shiftnet.AppHosts;
using Shiftnet.Apps;
using Shiftnet.Modules;

namespace Shiftnet
{
    public class Desktop : Scene
    {
        private Texture2D _wallpaper;
        private float _wallFade;
        private Texture2D _newWallpaper;
        private double _wallFadeTime;
        private double _wallFadeDuration = 1;
        private StackPanel _tabs;
        private Box _mainContent;

        // windows
        private SettingsWindow _settingsWindow;
        
        // gui elements
        private TextBlock _time = null;
        private Button _appLauncherOpen = null;
        private Button _settingsOpen = null;
        private Button _leave;
        private ConsoleControl _console;
        private Shell _shell;

        public SettingsService SettingsService
            => GetModule<SettingsService>();
        
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

            _tabs = Gui.FindById<StackPanel>("tabs");
            _mainContent = Gui.FindById<Box>("content");
            
            SetWallpaper("Backgrounds/rainyskies");
            
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
            _shell = new Shell(this, _console, true);
            _shell.Start();

        }

        public void ShutDown()
        {
            SceneSystem.GoToScene<MainMenu>();
        }

        public IShiftAppHost CreateAppHost(AppInformationAttribute appInfo)
        {
            // TODO: app hosts for Feed Area and Main Area.
            return OpenWindow<ShiftAppWindow>().LinkToDesktop(this);
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
            if (_newWallpaper != null)
            {
                _wallFadeTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_wallFadeTime >= _wallFadeDuration)
                {
                    _wallpaper = _newWallpaper;
                    _newWallpaper = null;
                    _wallFadeTime = 0;
                }

                _wallFade = MathHelper.Clamp((float) (_wallFadeTime / _wallFadeDuration), 0, 1);
            }
            
            _time.Text = DateTime.Now.ToShortTimeString();

            base.OnUpdate(gameTime);
        }

        protected override void OnUnload()
        {
            _shell.Stop();
            base.OnUnload();
        }

        [Exec("desktop.setWallpaper")]
        public void SetWallpaper(string wallpaper)
        {
            _newWallpaper = App.GameLoop.Content.Load<Texture2D>(wallpaper);
        }
        
        protected override void OnDraw(GameTime gameTime, SpriteRocket2D renderer)
        {
            base.OnDraw(gameTime, renderer);

            var color = Color.White;
            var darken = SettingsService.EnableDarkTheme;
            if (darken)
                color = color.Darken(0.15f);

            renderer.Begin();
            if (_wallpaper != null)
            {
                renderer.FillRectangle(Gui.BoundingBox, color * (1 - _wallFade), _wallpaper);
            }

            if (_newWallpaper != null)
            {
                renderer.FillRectangle(Gui.BoundingBox, color * _wallFade, _newWallpaper);
            }

            renderer.End();
        }
    }
}
