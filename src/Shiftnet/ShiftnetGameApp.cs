using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Settings;

namespace Shiftnet
{
    public class ShiftnetGameApp : App
    {
        private SceneSystem SceneSystem
            => GetModule<SceneSystem>();

        private SettingsService Settings
            => GetModule<SettingsService>();
        
        protected override void OnLoad()
        {
            SceneSystem.GoToScene<MainMenu>(null);
            
            #if DEBUG
            Settings.ShowDebugLogs = false;
#endif
        }
    }
}