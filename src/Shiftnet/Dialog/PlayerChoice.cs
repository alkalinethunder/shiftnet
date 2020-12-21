namespace Shiftnet.Dialog
{
    public class PlayerChoice
    {
        public string Id { get; }
        public string Label { get; }

        public PlayerChoice(string id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}