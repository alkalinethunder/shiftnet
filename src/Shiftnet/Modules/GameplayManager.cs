using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.SaveGame;

namespace Shiftnet.Modules
{
    [RequiresModule(typeof(SaveSystem))]
    public class GameplayManager : EngineModule
    {
        private SaveSystem SaveSystem
            => GetModule<SaveSystem>();
        
        
    }
}