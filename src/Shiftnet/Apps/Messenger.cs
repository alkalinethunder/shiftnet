using System;
using System.Net.Mime;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using Microsoft.Xna.Framework;
using Shiftnet.Dialog;
using Shiftnet.Modules;
using Shiftnet.Saves;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlkalineThunder.Pandemic.Input;

namespace Shiftnet.Apps
{
    [AppInformation("Messenger", "Chat with people.", UserCloseable = true, SingleInstance = false, PlayerOnly = true)]
    public class Messenger : ShiftApp
    {
        private StackPanel _choices;
        private List<string> _typers = new List<string>();
        private DialogPlayer _dialog;
        private Func<Control> MakeNpcGui;
        private Func<Control> MakePlayerGui;
        private StackPanel _messages;
        private ContactInformation _contact;
        private TextBlock _typing;
        private Box _dnd;

        private GameplayManager GameplayManager
            => ShiftOS.GetModule<GameplayManager>();
        
        protected override void Main()
        {
            _contact = LaunchProperties.GetValue<ContactInformation>("Contact");
            
            MakeNpcGui = GuiBuilder.MakeBuilderFunction(this, "layout/component/message-npc.gui");
            MakePlayerGui = GuiBuilder.MakeBuilderFunction(this, "layout/component/message-player.gui");

            Title = $"Messenger: {_contact.Username}";
            Gui.AddChild(GuiBuilder.Build(this, "layout/app/messenger.gui"));

            Gui.FindById<TextBlock>("fullname").Text = _contact.FullName;
            Gui.FindById<TextBlock>("username").Text = _contact.Username;

            _dnd = Gui.FindById<Box>("dnd");
            
            _messages = Gui.FindById<StackPanel>("messages");

            _typing = Gui.FindById<TextBlock>("typing");

            _typing.Visible = false;
            
            GameplayManager.DoNotDisturbChanged += (o, a) =>
            {
                _dnd.Visible = GameplayManager.DoNotDisturb;
                _dnd.LayoutVisible = GameplayManager.DoNotDisturb;
            };
            
            _dnd.Visible = GameplayManager.DoNotDisturb;
            _dnd.LayoutVisible = GameplayManager.DoNotDisturb;

            _choices = Gui.FindById<StackPanel>("choices");
            
            _dialog = _contact.GetDialogPlayer();
            _dialog.TypingStarted += DialogOnTypingStarted;
            _dialog.TypingEnded += DialogOnTypingEnded;
            _dialog.MessageSent += DialogOnMessageSent;
            
            _dialog.ChoiceNeeded += DialogOnChoiceNeeded;
            _dialog.ChoiceFinished += DialogOnChoiceFinished;
        }

        private void DialogOnChoiceFinished(object? sender, EventArgs e)
        {
            _choices.Visible = false;
            _choices.Clear();
        }

        private void DialogOnChoiceNeeded(object? sender, ChoiceNeededEventArgs e)
        {
            _choices.Visible = true;
            _choices.Clear();

            foreach (var choice in e.Choices)
            {
                var message = MakePlayerGui();

                message.FindById<TextBlock>("text").Text = choice.Label;
                message.FindById<TextBlock>("date").Text = "Click to choose.";

                message.Tag = choice;

                _choices.AddChild(message);
                
                message.Click += ChoiceClicked;
            }
        }

        private void ChoiceClicked(object? sender, MouseButtonEventArgs e)
        {
            if (sender is Control control)
            {
                if (control.Tag is PlayerChoice choice)
                {
                    _dialog.SubmitChoice(choice);
                }
            }
        }

        private void DialogOnMessageSent(object? sender, DialogMessageEventArgs e)
        {
            MakeMessage(e.DialogMessage.Sent, e.DialogMessage.Text, !e.IsPlayer);
        }

        private void DialogOnTypingEnded(object? sender, TypingEventArgs e)
        {
            _typers.Remove(e.Username);
            UpdateTypers();
        }
        
        private void DialogOnTypingStarted(object? sender, TypingEventArgs e)
        {
            _typers.Add(e.Username);
            UpdateTypers();
        }

        private void UpdateTypers()
        {
            if (_typers.Any())
            {
                var sb = new StringBuilder();

                for (var i = 0; i < _typers.Count; i++)
                {
                    var typer = _typers[i];

                    if (i > 0)
                    {
                        if (i == _typers.Count - 1)
                        {
                            sb.Append(" and ");
                        }
                        else
                        {
                            sb.Append(", ");
                        }
                    }

                    sb.Append(typer);
                }

                if (_typers.Count > 1)
                {
                    sb.Append(" are ");
                }
                else
                {
                    sb.Append(" is ");
                }

                sb.Append("typing...");
                _typing.Text = sb.ToString();
                _typing.Visible = true;
            }
            else
            {
                _typing.Visible = false;
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            _dialog.Update(gameTime);
        }

        private void MakeMessage(DateTime time, string message, bool isNPC)
        {
            var gui = (isNPC ? MakeNpcGui : MakePlayerGui)(); // <-------- RIGHT HERE

            gui.FindById<TextBlock>("text").Text = message;
            gui.FindById<TextBlock>("date").Text = time.ToString();

            _messages.AddChild(gui);
        }
    }
}