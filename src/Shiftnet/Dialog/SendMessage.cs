using Microsoft.Xna.Framework;
using Shiftnet.Saves;

namespace Shiftnet.Dialog
{
    public class SendMessage : DialogAction
    {
        private double _wpm = 15;
        private double _typeDelay;
        private double _cooldown = 2;
        
        private string _fullname;
        private string _username;
        private string _avatar;
        private string _message;
        private bool _isPlayer;

        private int _stage;
        
        public SendMessage(string avatar, string username, string fullname, bool isPlayer, string message)
        {
            _avatar = avatar;
            _username = username;
            _fullname = fullname;
            _message = message;
            _isPlayer = isPlayer;
        }

        public SendMessage(Npc npc, string message) : this(npc.Avatar, npc.Username, npc.FullName, npc.Id == -1, message)
        {
            
        }

        protected override bool OnUpdate(GameTime gameTime)
        {
            switch (_stage)
            {
                case 0:
                    DialogPlayer.StartTyping(_username);
                    _typeDelay = _message.Length / (_wpm * 60);
                    _stage++;
                    break;
                case 1:
                    _typeDelay -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (_typeDelay <= 0)
                    {
                        DialogPlayer.EndTyping(_username);
                        DialogPlayer.SendMessage(new DialogMessage
                        {
                            Avatar = _avatar,
                            Text = _message,
                            Username = _username,
                            FullName = _fullname
                        }, _isPlayer);
                        _stage++;
                    }
                    break;
                case 2:
                    _cooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (_cooldown <= 0)
                        return true;
                    break;
            }

            return false;
        }
    }
}