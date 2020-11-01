using AlkalineThunder.Pandemic.Scenes;

namespace Shiftnet
{
    public static class InfoboxExtensions
    {
        public static void ShowInfobox(this Scene scene, string title, string message)
        {
            var info = scene.OpenWindow<Infobox>();
            info.Title = title;
            info.Message = message;
        }

    }
}