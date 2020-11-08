using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shiftnet
{
    public static class Paths
    {
        public static string[] Split(string path)
        {
            var word = "";
            var parts = new List<string>();

            foreach (var letter in path)
            {
                if (letter == '/')
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        parts.Add(word);
                        word = "";
                    }

                    continue;
                }

                word += letter;
            }

            if (!string.IsNullOrWhiteSpace(word))
            {
                parts.Add(word);
                word = "";
            }
            
            return parts.ToArray();
        }

        public static string Combine(params string[] parts)
        {
            if (!parts.Any())
                return "/";
            
            var path = "";

            foreach (var part in parts)
            {
                var trimmed = part;

                while (trimmed.EndsWith("/"))
                    trimmed = trimmed.Substring(0, trimmed.LastIndexOf("/"));

                if (!trimmed.StartsWith("/"))
                    path += "/";

                path += trimmed;
            }

            return path;
        }

        public static string GetAbsolute(string path)
        {
            var parts = Split(path);
            var stack = new Stack<string>();

            foreach (var part in parts)
            {
                if (part == ".")
                    continue;
                
                if (part == "..")
                {
                    if (stack.Any())
                        stack.Pop();
                    continue;
                }

                stack.Push(part);
            }

            return Combine(stack.ToArray());
        }
    }
}