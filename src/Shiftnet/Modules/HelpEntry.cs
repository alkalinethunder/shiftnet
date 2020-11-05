namespace Shiftnet.Modules
{
    public struct HelpEntry
    {
        public string Name;
        public string Description;

        public HelpEntry(string name, string desc)
        {
            Name = name;
            Description = desc;
        }
    }
}