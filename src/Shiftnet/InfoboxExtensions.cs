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

        public static void AskForText(this Scene scene, string title, string message, InfoboxTextCallback callback,
            params InfoboxValidator[] validators)
        {
            var info = scene.OpenWindow<Infobox>();
            info.Title = title;
            info.Message = message;
            info.TextPromptEnabled = true;
            info.TextValidators = validators;
            info.TextCallback = callback;
        }
    }

    public delegate bool InfoboxValidator(string text, out string errorMessage);

    public delegate void InfoboxTextCallback(InfoboxResult result, string text);
}