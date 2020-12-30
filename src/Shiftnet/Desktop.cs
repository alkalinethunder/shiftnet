using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Rendering;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shiftnet.AppHosts;
using Shiftnet.Apps;
using Shiftnet.Controls;
using Shiftnet.Modules;

namespace Shiftnet
{
    public class Desktop : Scene
    {
        private Func<Control> MakeStatusIcon;
        
        private Texture2D _wallpaper;
        private float _wallFade;
        private Texture2D _newWallpaper;
        private double _wallFadeTime;
        private double _wallFadeDuration = 1;
        private StackPanel _tabs;
        private StackPanel _socialTabs;
        private IShiftOS _currentOS;
        private Box _mainContent;
        private List<TabbedAppHost> _mainApps = new List<TabbedAppHost>();
        private List<TabbedAppHost> _socialApps = new List<TabbedAppHost>();
        private StackPanel _mainTabs;
        private SwitcherPanel _mainArea;
        private SwitcherPanel _socialSwitcher;
        private Box _appLauncherBox;
        private StackPanel _userIcons;
        private StackPanel _infoWidgets;
        
        private AppsModule AppsModule => GetModule<AppsModule>();
        
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
            MakeStatusIcon = GuiBuilder.MakeBuilderFunction(this, "layout/component/statusIcon.gui");
            
            _currentOS = Properties.GetValue<IShiftOS>("os", null)
                         ?? throw new InvalidOperationException(
                             "An IShiftOS implementation is needed in the 'os' property of this Scene.");
            
            Gui.AddChild(GuiBuilder.Build(this, "layout/desktop.gui"));

            _infoWidgets = Gui.FindById<StackPanel>("infoWidgets");
            _time = Gui.FindById<TextBlock>("time");
            _appLauncherOpen = Gui.FindById<Button>("apps");
            _settingsOpen = Gui.FindById<Button>("settings");
            _leave = Gui.FindById<Button>("leave");

            _appLauncherBox = Gui.FindById<Box>("appLauncher");
            
            _settingsOpen.Click += SettingsOpenOnClick;
            _appLauncherOpen.Click+= AppLauncherOpenOnClick;
            _leave.Click += LeaveOnClick;
            
            _console = Gui.FindById<ConsoleControl>("console");
            StartShell();

            _tabs = Gui.FindById<StackPanel>("tabs");
            _mainContent = Gui.FindById<Box>("content");

            _mainArea = Gui.FindById<SwitcherPanel>("main-switcher");
            _mainTabs = Gui.FindById<StackPanel>("tabs-main");
            
            SetWallpaper("Backgrounds/contest_spotlight");
            
            base.OnLoad();

            _appLauncherBox.Parent.Visible = false;
            _appLauncherBox.LostFocus += AppLauncherBoxOnLostFocus;

            _socialTabs = Gui.FindById<StackPanel>("tabs-social");
            _socialSwitcher = Gui.FindById<SwitcherPanel>("switcher-social");

            _userIcons = Gui.FindById<StackPanel>("userIcons");
            
            LaunchStartupApps();
        }

        private void LaunchStartupApps()
        {
            // Are we a player?
            var isPlayer = this.CurrentOS.IsPlayer;

            // Get all app launchers
            //  - that can be launched by the current user
            // - and that are startup apps.
            var startupApps = this.AppsModule.AvailableAppLaunchers
                .Where(x => isPlayer || !x.PlayerOnly)
                .Where(x => x.Startup);

            // LAUNCH THEN, BITCH!
            foreach (var launcher in startupApps)
            {
                launcher.Launch(null, this, this.CurrentOS.Home);
            }
        }

        private void AppLauncherOpenOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (!_appLauncherBox.Visible)
            {
                _appLauncherBox.Parent.Visible = true;
                SceneSystem.SetFocus(_appLauncherBox);
            }
        }

        private void AppLauncherBoxOnLostFocus(object? sender, FocusEventArgs e)
        {
            _appLauncherBox.Parent.Visible = false;
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
                SceneSystem.GoToScene<MainMenu>(null);
            }
        }

        private void StartShell()
        {
            _shell = new Shell(this, _console, true);
            _shell.Start();

        }

        public void NotifyTabbedAppHostClosed(TabbedAppHost host)
        {
            if (_mainApps.Contains(host))
                _mainApps.Remove(host);
        }
        
        public void ShutDown()
        {
            SceneSystem.GoToScene<MainMenu>(null);
        }

        public IShiftOS CurrentOS
            => _currentOS;
        
        public IShiftAppHost CreateAppHost(AppInformationAttribute appInfo)
        {
            if (appInfo.DisplayTarget == DisplayTarget.Windows)
            {
                return OpenWindow<ShiftAppWindow>().LinkToDesktop(this);
            }
            else if (appInfo.DisplayTarget == DisplayTarget.InfoWidgets)
            {
                var canvas = new CanvasPanel();
                var host = new InfoWidgetHost(this, canvas);
                
                _infoWidgets.AddChild(canvas);
                return host;
            }
            else if (appInfo.DisplayTarget == DisplayTarget.StatusIcon)
            {
                var icon = MakeStatusIcon();
                var canvas = new CanvasPanel();
                var host = new StatusIconHost(this, icon, canvas);

                _userIcons.AddChild(icon);

                return host;
            }
            else
            {
                var tab = new PanelTab(appInfo.UserCloseable);
                var canvas = new CanvasPanel();
                var tabHost = new TabbedAppHost(this, tab, canvas);
                
                switch (appInfo.DisplayTarget)
                {
                    case DisplayTarget.Feed:
                        _socialTabs.AddChild(tab);
                        _socialSwitcher.AddChild(canvas);
                        _socialApps.Add(tabHost);
                        break;
                    default:
                        _mainTabs.AddChild(tab);
                        _mainArea.AddChild(canvas);
                        _mainApps.Add(tabHost);
                        break;
                }

                return tabHost;
            }
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

            // update active tab for main area.
            foreach (var tab in _mainApps)
            {
                tab.UpdateActiveTab();
            }
            
            base.OnUpdate(gameTime);
        }

        protected override void OnUnload()
        {
            _shell.Stop();
            base.OnUnload();
        }
        
        [Exec("setWallpaper")]
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
