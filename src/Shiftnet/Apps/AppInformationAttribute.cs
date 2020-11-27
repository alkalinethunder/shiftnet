using System;

namespace Shiftnet.Apps
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppInformationAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Command { get; set; }
        public bool UserCloseable { get; set; } = true;
        public DisplayTarget DisplayTarget { get; }
        public bool SingleInstance { get; set; } = true;
        public bool PlayerOnly { get; set; } = false;
        
        public AppInformationAttribute(string name, string desc, DisplayTarget target = Apps.DisplayTarget.Default)
        {
            DisplayTarget = target;
            Name = name;
            Description = desc;
        }
    }
}