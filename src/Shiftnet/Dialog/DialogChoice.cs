using System.Collections.Generic;

namespace Shiftnet.Dialog
{
    public class DialogChoice
    {
        private List<DialogAction> _actions = new List<DialogAction>();
        
        public string Id { get; }
        public string Label { get; }

        public DialogAction[] Actions => _actions.ToArray();
        
        public DialogChoice(string id, string label)
        {
            Id = id;
            Label = label;
        }

        public void AddAction(DialogAction action)
        {
            _actions.Add(action);
        }
    }
}