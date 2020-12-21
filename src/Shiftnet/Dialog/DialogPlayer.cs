using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using DiscordRPC.Events;
using Microsoft.Xna.Framework;
using Shiftnet.Modules;
using Shiftnet.Saves;
using IGameComponent = AlkalineThunder.Pandemic.IGameComponent;

namespace Shiftnet.Dialog
{
    public class DialogPlayer : IGameComponent
    {
        private DialogBranch _currentBranch;
        private Stack<DialogBranch> _BranchStack = new Stack<DialogBranch>();
        
        private List<DialogBranch> _branches = new List<DialogBranch>();

        private bool _completed;
        private const double _typeSpeed = 15;
        private const double _typeCooldown = 1;
        private GameplayManager _gameplayManager;
        private ConversationInfo _convo;

        private double _cooldownTimer = 0;
        private double _typeTimer = 0;
        private bool _typing;
        private Npc _typer;
        private string _message = "This is a test.";
        private Npc[] _npcs;

        private bool _choicesReady = false;
        
        public DialogPlayer(GameplayManager gameplayManager, ConversationInfo resource)
        {
            _convo = resource;
            _gameplayManager = gameplayManager;

            _npcs = _convo.Members.Keys.Select(x => _gameplayManager.GetNpcById(x)).ToArray();
            _typer = _npcs.First();

            ReadConversationData();
        }

        private void ReadConversationData()
        {
            using var resourceStream = GetType().Assembly.GetManifestResourceStream(_convo.ConversationId);
            
            var xmlReader = new XmlDocument();
            xmlReader.Load(resourceStream);

            var convoNode = xmlReader.DocumentElement;
            
            if (convoNode.Name != "Conversation")
                throw new InvalidOperationException("Invalid conversation resource.");

            var rootBranch = convoNode["Start"];
            var subBranches = convoNode["Branches"];

            ReadBranch(rootBranch);

            if (subBranches != null)
            {
                foreach (XmlNode branch in subBranches.ChildNodes)
                    ReadBranch(branch);
            }

            _currentBranch = _branches.First();
        }

        private void ReadBranch(XmlNode branch)
        {
            var id = branch.Name == "Start" ? "root" : branch.Attributes["Id"].Value;
            
            var loadedBranch = new DialogBranch(id);
            
            var actions = branch["Actions"];
            var choices = branch["Choices"];

            foreach (var action in ReadActions(actions))
            {
                action.DialogPlayer = this;
                loadedBranch.AddAction(action);
            }

            foreach (var choice in ReadChoices(choices))
            {
                loadedBranch.AddChoice(choice);
            }
            
            _branches.Add(loadedBranch);
        }

        private IEnumerable<DialogChoice> ReadChoices(XmlNode node)
        {
            foreach (XmlNode choiceNode in node.ChildNodes)
            {
                if (choiceNode.Name != "Choice")
                    continue;

                var id = choiceNode.Attributes["Id"].Value;
                var label = choiceNode["Label"].InnerText.Trim();

                var choice = new DialogChoice(id, label);

                foreach (var action in ReadActions(choiceNode["Actions"]))
                {
                    action.DialogPlayer = this;
                    choice.AddAction(action);
                }

                yield return choice;
            }
        }

        private IEnumerable<DialogAction> ReadActions(XmlNode actions)
        {
            foreach (XmlNode childNode in actions.ChildNodes)
            {
                if (childNode.Name == "SendMessage")
                {
                    var from = childNode.Attributes["From"].Value;
                    var text = childNode.InnerText.Trim();

                    var npcId = _convo.Members.First(
                        x => x.Value == from).Key;
                    var npc = _gameplayManager.GetNpcById(npcId);

                    yield return new SendMessage(npc, text);
                }
            }
        }

        public void SubmitChoice(PlayerChoice choice)
        {
            _currentBranch.Choose(choice.Id);
        }
        
        public void Update(GameTime gameTime)
        {
            if (!_completed)
            {
                if (_currentBranch.Update(gameTime))
                {
                    if (_BranchStack.Count > 0)
                    {
                        _currentBranch = _BranchStack.Pop();
                    }
                    else
                    {
                        Complete();
                    }
                }
                else
                {
                    if (_choicesReady != _currentBranch.ChoicesReady)
                    {
                        if (_currentBranch.ChoicesReady)
                        {
                            var choices = _currentBranch.Choices.Select(x => new PlayerChoice(x.Id, x.Label)).ToArray();
                            ChoiceNeeded?.Invoke(this, new ChoiceNeededEventArgs(choices));
                        }
                        else
                        {
                            ChoiceFinished?.Invoke(this, EventArgs.Empty);
                        }

                        _choicesReady = _currentBranch.ChoicesReady;
                    }
                }
            }
        }

        private void Complete()
        {
            _completed = true;
        }

        public void SendMessage(DialogMessage dialogMessage, bool isPlayer)
        {
            MessageSent?.Invoke(this, new DialogMessageEventArgs(dialogMessage, isPlayer));
        }
        
        public void EndTyping(string username)
        {
            TypingEnded?.Invoke(this, new TypingEventArgs(username));
        }
        
        public void StartTyping(string username)
        {
            TypingStarted?.Invoke(this, new TypingEventArgs(username));
        }
        
        public event EventHandler<DialogMessageEventArgs> MessageSent;
        public event EventHandler<TypingEventArgs> TypingStarted;
        public event EventHandler<TypingEventArgs> TypingEnded;
        public event EventHandler ChoiceFinished;
        public event EventHandler<ChoiceNeededEventArgs> ChoiceNeeded;
    }
}