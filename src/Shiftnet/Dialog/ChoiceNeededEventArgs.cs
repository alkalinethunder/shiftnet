using System;

namespace Shiftnet.Dialog
{
    public class ChoiceNeededEventArgs : EventArgs
    {
        public PlayerChoice[] Choices;

        public ChoiceNeededEventArgs(PlayerChoice[] choices)
        {
            Choices = choices;
        }
    }
}