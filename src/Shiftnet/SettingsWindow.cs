using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Settings;
using AlkalineThunder.Pandemic.Windowing;

namespace Shiftnet
{
    public class SettingsWindow : Window
    {
        private SettingsService Settings
            => SceneSystem.GetModule<SettingsService>();
        
        private SelectList _resolutions;
        
        
        protected override void OnInitialize()
        {
            var gui = GuiBuilder.Build(this, "layout/settings.gui");

            _resolutions = gui.FindById<SelectList>("resolutions");
            
            foreach (var mode in Settings.AvailableResolutions)
                _resolutions.Add(mode);

            _resolutions.SelectedIndex = _resolutions.Find(Settings.ActiveResolution);
            
            Gui.AddChild(gui);
        }
    }
}