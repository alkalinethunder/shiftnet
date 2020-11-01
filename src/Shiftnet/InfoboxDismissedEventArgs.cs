using System;

namespace Shiftnet
{
    public class InfoboxDismissedEventArgs : EventArgs
    {
        public InfoboxResult Result { get; }

        public InfoboxDismissedEventArgs(InfoboxResult result)
        {
            Result = result;
        }
    }
}