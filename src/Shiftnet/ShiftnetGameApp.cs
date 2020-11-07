using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Scenes;
using Plex.Engine;

namespace Shiftnet
{
    public class ShiftnetGameApp : App
    {
        private SceneSystem SceneSystem
            => GetModule<SceneSystem>();
        
        protected override void OnLoad()
        {
            SceneSystem.GoToScene<MainMenu>(null);
        }
    }
}