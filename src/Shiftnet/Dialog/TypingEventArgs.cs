using System;

namespace Shiftnet.Dialog
{
    public class TypingEventArgs : EventArgs
    {
        public string Username { get; }

        public TypingEventArgs(string username)
        {
            Username = username;
        }
    }
}