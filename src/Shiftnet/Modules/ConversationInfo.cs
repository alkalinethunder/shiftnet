namespace Shiftnet.Modules
{
    public class ConversationInfo
    {
        public string Id { get; set; }
        public int[] Members { get; set; }
        public string ConversationId { get; set; }
        public EncounterType EncounterType { get; set; }
    }

    public enum EncounterType
    {
        Scripted,
        Random
    }
}