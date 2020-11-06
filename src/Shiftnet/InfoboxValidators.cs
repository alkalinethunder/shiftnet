using System.Linq;

namespace Shiftnet
{
    public static class InfoboxValidators
    {
        public static bool NonEmpty(string text, out string error)
        {
            error = string.Empty;
            
            if (string.IsNullOrEmpty(text))
            {
                error = "Input must not be empty.";
                return false;
            }

            return true;
        }
        
        public static bool NonWhite(string text, out string error)
        {
            return NonEmpty(text.Trim(), out error);
        }

        public static bool Integer(string text, out string error)
        {
            error = string.Empty;
            
            if (!int.TryParse(text, out var value))
            {
                error = "Input must be a whole number.";
                return false;
            }

            return true;
        }

        public static bool NoSpaces(string text, out string error)
        {
            error = string.Empty;

            if (text.Any(char.IsWhiteSpace))
            {
                error = "Text cannot contain white space.";
                return false;
            }

            return true;
        }
    }
}