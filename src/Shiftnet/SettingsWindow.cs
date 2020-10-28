using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using AlkalineThunder.Pandemic.Settings;
using AlkalineThunder.Pandemic.Windowing;

namespace Shiftnet
{
    public class SettingsWindow : Window
    {
        private FullScreenMode _fullscreenMode;
        
        private SettingsService Settings
            => SceneSystem.GetModule<SettingsService>();
        
        private SelectList _resolutions;
        private CheckBox _vsync;
        private CheckBox _blurs;
        private CheckBox _transparency;
        private SliderBar _masterVolume;
        private SliderBar _sfx;
        private SliderBar _music;
        private CheckBox _darkMode;
        private CheckBox _colorblind;
        private CheckBox _largeFont;
        private CheckBox _swapMouse;
        private Button _fullscreen;
        private Button _borderless;
        private Button _windowed;
        private CheckBox _discordPresence;
        private Button _ok;
        private Button _cancel;
        
        protected override void OnInitialize()
        {
            var gui = GuiBuilder.Build(this, "layout/settings.gui");

            _resolutions = gui.FindById<SelectList>("resolutions");

            _vsync = gui.FindById<CheckBox>("vsync");
            _blurs = gui.FindById<CheckBox>("blurs");
            _transparency = gui.FindById<CheckBox>("transparencyEffects");
            _fullscreen = gui.FindById<Button>("fullscreen");
            _borderless = gui.FindById<Button>("borderless");
            _windowed = gui.FindById<Button>("windowed");
            _darkMode = gui.FindById<CheckBox>("darkMode");
            _largeFont = gui.FindById<CheckBox>("largerFont");
            _colorblind = gui.FindById<CheckBox>("colorblind");
            _swapMouse = gui.FindById<CheckBox>("swapMouseButtons");
            _masterVolume = gui.FindById<SliderBar>("masterVolume");
            _music = gui.FindById<SliderBar>("music");
            _sfx = gui.FindById<SliderBar>("sfx");
            _discordPresence = gui.FindById<CheckBox>("discordPresence");

            _ok = gui.FindById<Button>("ok");
            _cancel = gui.FindById<Button>("cancel");
            
            PopulateSettings();
            
            Gui.AddChild(gui);
            
            _ok.Click += OkOnClick;
            _cancel.Click += CancelOnClick;
            
            _fullscreen.Click += FullscreenOnClick;
            _borderless.Click += BorderlessOnClick;
            _windowed.Click += WindowedOnClick;
        }

        private void WindowedOnClick(object? sender, MouseButtonEventArgs e)
        {
            SetFullScreenMode(FullScreenMode.Windowed);
        }

        private void BorderlessOnClick(object? sender, MouseButtonEventArgs e)
        {
            SetFullScreenMode(FullScreenMode.Borderless);
        }

        private void FullscreenOnClick(object? sender, MouseButtonEventArgs e)
        {
            SetFullScreenMode(FullScreenMode.FullScreen);
        }

        private void SetFullScreenMode(FullScreenMode mode)
        {
            _fullscreenMode = mode;
            SetWindowModeButtonColors();
        }
        
        private void CancelOnClick(object? sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OkOnClick(object? sender, MouseButtonEventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void SaveSettings()
        {
            // vsync
            Settings.EnableVSync = _vsync.IsChecked;
            
            // gfx effects
            Settings.EnableBlurs = _blurs.IsChecked;
            Settings.EnableTerminalTransparency = _transparency.IsChecked;
            
            // accessibility
            Settings.EnableDarkTheme = _darkMode.IsChecked;
            Settings.SetFontSize(_largeFont.IsChecked ? FontSizeAdjustment.Large : FontSizeAdjustment.Normal);
            
            // input settings
            Settings.SwapPrimaryMouseButton = _swapMouse.IsChecked;

            // set audio settings
            Settings.SetValue("audio.master", _masterVolume.Value);
            Settings.SetValue("audio.music", _music.Value);
            Settings.SetValue("audio.sfx", _sfx.Value);

            // Set Shiftnet settings now.
            Settings.SetValue("colorblind", _colorblind.IsChecked);
            Settings.SetValue("discord.presence", _discordPresence.IsChecked);
            
            // Set display mode now.
            Settings.ApplyResolution(_resolutions.SelectedItem);
            Settings.FullScreenMode = _fullscreenMode;
        }
        
        private void PopulateSettings()
        {
            _resolutions.Clear();
            foreach(var mode in Settings.AvailableResolutions)
                _resolutions.Add(mode);
            _resolutions.SelectedIndex = _resolutions.Find(Settings.ActiveResolution);

            // engine-wide settings
            _vsync.IsChecked = Settings.EnableVSync;
            _blurs.IsChecked = Settings.EnableBlurs;
            _transparency.IsChecked = Settings.EnableTerminalTransparency;
            _darkMode.IsChecked = Settings.EnableDarkTheme;
            _largeFont.IsChecked = Settings.FontSizeAdjustment == FontSizeAdjustment.Large;
            _swapMouse.IsChecked = Settings.SwapPrimaryMouseButton;
            _masterVolume.Value = Settings.GetValue("audio.master", 1);
            _music.Value = Settings.GetValue("audio.music", 1);
            _sfx.Value = Settings.GetValue("audio.sfx", 1);

            // shiftnet settings
            _discordPresence.IsChecked = Settings.GetValue("discord.presence", true);
            _colorblind.IsChecked = Settings.GetValue("colorblind", false);
            
            // fullscreen mode
            _fullscreenMode = Settings.FullScreenMode;
            SetWindowModeButtonColors();
        }

        private void SetWindowModeButtonColors()
        {
            _fullscreen.ButtonColor = _fullscreenMode == FullScreenMode.FullScreen
                ? ControlColor.Primary
                : ControlColor.Secondary;
            _borderless.ButtonColor = _fullscreenMode == FullScreenMode.Borderless
                ? ControlColor.Primary
                : ControlColor.Secondary;
            _windowed.ButtonColor = _fullscreenMode == FullScreenMode.Windowed
                ? ControlColor.Primary
                : ControlColor.Secondary;
        }
    }
}