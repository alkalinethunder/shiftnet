namespace Shiftnet.Saves
{
    public class EmailMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int Sender { get; set; }
        public int Recipient { get; set; }
        public int ReplyToId { get; set; }
        public bool WasForwarded { get; set; }
    }
}