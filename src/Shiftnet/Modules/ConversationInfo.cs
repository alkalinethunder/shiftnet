using System.Collections.Generic;

namespace Shiftnet.Modules
{
    public class ConversationInfo
    {
        public string Id { get; set; }
        public Dictionary<int, string> Members { get; set; }
        public string ConversationId { get; set; }
        public EncounterType EncounterType { get; set; }
    }

    public enum EncounterType
    {
        Scripted,
        Random
    }
}