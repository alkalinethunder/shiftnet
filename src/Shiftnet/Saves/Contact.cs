namespace Shiftnet.Saves
{
    public class Contact
    {
        public int Id { get; set; }
        public string ConversationId { get; set; }
        public ContactType ContactType { get; set; }
    }

    public enum ContactType
    {
        DirectMessage,
        GroupChat
    }
}