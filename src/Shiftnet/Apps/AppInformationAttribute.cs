using System;

namespace Shiftnet.Apps
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppInformationAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public DisplayTarget DisplayTarget { get; }
        public bool SingleInstance { get; set; } = true;

        public AppInformationAttribute(string name, string desc, DisplayTarget target = Apps.DisplayTarget.Default)
        {
            DisplayTarget = target;
            Name = name;
            Description = desc;
        }
    }
}