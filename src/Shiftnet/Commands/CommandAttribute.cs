using System;

namespace Shiftnet.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public CommandAttribute(string name, string desc = "")
        {
            Name = name;
            Description = desc;
        }
    }
}