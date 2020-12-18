using System;
using System.Net.Mime;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using Shiftnet.Modules;
using Shiftnet.Saves;

namespace Shiftnet.Apps
{
    [AppInformation("Messenger", "Chat with people.", UserCloseable = true, SingleInstance = false, PlayerOnly = true)]
    public class Messenger : ShiftApp
    {
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

            for (var i = 0; i < 200; i++)
            {
                MakeMessage(DateTime.Now, "Test message " + (i + 1).ToString(), i % 2 == 0);
            }
        }

        private void MakeMessage(DateTime time, string message, bool isNPC)
        {
            var gui = (isNPC ? MakeNpcGui : MakePlayerGui)();

            gui.FindById<TextBlock>("text").Text = message;
            gui.FindById<TextBlock>("date").Text = time.ToString();

            _messages.AddChild(gui);
        }
    }
}