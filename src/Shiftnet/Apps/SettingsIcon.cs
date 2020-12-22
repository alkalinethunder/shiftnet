using System;
using AlkalineThunder.Pandemic.Scenes;

namespace Shiftnet.Apps
{
    [AppInformation("System Status Icon: Settings", "Opens the game settings dialog.", DisplayTarget.StatusIcon, PlayerOnly = true, Startup = true)]
    public class SettingsIcon : StatusApplet
    {
        private SettingsWindow _settingsWindow;

        protected override void OnClick()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = (ShiftOS as Scene).OpenWindow<SettingsWindow>();
                _settingsWindow.Closed += SettingsWindowOnClosed;
            }
        }

        private void SettingsWindowOnClosed(object? sender, EventArgs e)
        {
            _settingsWindow = null;
        }
    }
}