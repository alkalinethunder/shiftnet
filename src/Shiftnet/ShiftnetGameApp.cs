using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Scenes;

namespace Shiftnet
{
    public class ShiftnetGameApp : App
    {
        private SceneSystem SceneSystem
            => GetModule<SceneSystem>();
        
        protected override void OnLoad()
        {
            SceneSystem.GoToScene<MainMenu>();
        }
    }
}