using System;

namespace Shiftnet.Dialog
{
    public class DialogMessage
    {
        public string Avatar { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public DateTime Sent { get; set; } = DateTime.Now;
    }
}