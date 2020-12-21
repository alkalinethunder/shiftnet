using System;

namespace Shiftnet.Dialog
{
    public class DialogMessageEventArgs : EventArgs
    {
        public DialogMessage DialogMessage { get; }
        public bool IsPlayer { get; }
        
        public DialogMessageEventArgs(DialogMessage message, bool isPlayer)
        {
            IsPlayer = isPlayer;
            DialogMessage = message;
        }
    }
}