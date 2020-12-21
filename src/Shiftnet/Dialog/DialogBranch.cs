using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Shiftnet.Dialog
{
    public class DialogBranch
    {
        private bool _choicesReady;
        private int _currentAction = 0;
        private List<DialogAction> _actions = new List<DialogAction>();
        private List<DialogChoice> _choices = new List<DialogChoice>();
        private bool _choiceChosen;

        public bool ChoicesReady => _choicesReady && !_choiceChosen;

        public IEnumerable<DialogChoice> Choices => _choices;

        public void AddChoice(DialogChoice choice)
        {
            _choices.Add(choice);
        }
        
        public string Id { get; }

        public DialogBranch(string id)
        {
            Id = id;
        }

        public void Choose(string choiceId)
        {
            if (_choicesReady && !_choiceChosen)
            {
                var choice = _choices.First(x => x.Id == choiceId);
                _actions.AddRange(choice.Actions);
                _choiceChosen = true;
            }
        }
        
        public void AddAction(DialogAction action)
        {
            _actions.Add(action);
        }

        public bool Update(GameTime gameTime)
        {
            if (_currentAction < _actions.Count)
            {
                var act = _actions[_currentAction];
                
                if (act.Completed)
                {
                    _currentAction++;
                }
                else
                {
                    act.Update(gameTime);
                }

                return false;
            }
            else
            {
                if (!_choicesReady)
                {
                    _choicesReady = true;
                    return false;
                }

                if (!_choiceChosen)
                    return false;

                return true;
            }
        }
    }
}