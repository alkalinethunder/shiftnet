using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shiftnet.Dialog;
using Shiftnet.Modules;

namespace Shiftnet.Saves
{
    public class ContactInformation
    {
        private DialogPlayer _player;
        private GameplayManager _gameplayManager;
        private Contact _contact;
        private ConversationInfo _convo;
        
        public ContactInformation(GameplayManager gameplayManager, Contact contact)
        {
            _contact = contact;
            _gameplayManager = gameplayManager;
            _convo = _gameplayManager.GetConversationInfo(_contact.ConversationId);
        }

        protected IEnumerable<Npc> Npcs => _convo.Members.Keys.Select(x => _gameplayManager.GetNpcById(x));

        private string GetFullName()
        {
            var sb = new StringBuilder();

            foreach (var npc in Npcs)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(npc.FullName);
            }
            
            return sb.ToString();
        }

        public DialogPlayer GetDialogPlayer()
        {
            if (_player == null)
            {
                _player = new DialogPlayer(_gameplayManager, _convo);
            }

            return _player;
        }

        public string FullName => GetFullName();
        public string Username => Npcs.Count() > 1 ? "Group DM" : $"@{Npcs.First().Username}";
    }
}