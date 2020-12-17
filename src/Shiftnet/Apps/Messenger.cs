using System;
using System.Net.Mime;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;

namespace Shiftnet.Apps
{
    [AppInformation("Messenger", "Chat with people.", UserCloseable = true, SingleInstance = false, PlayerOnly = true)]
    public class Messenger : ShiftApp
    {
        private Func<Control> MakeNpcGui;
        private Func<Control> MakePlayerGui;
        private StackPanel _messages;
        
        protected override void Main()
        {
            MakeNpcGui = GuiBuilder.MakeBuilderFunction(this, "layout/component/message-npc.gui");
            MakePlayerGui = GuiBuilder.MakeBuilderFunction(this, "layout/component/message-player.gui");

            Title = "Messenger";
            Gui.AddChild(GuiBuilder.Build(this, "layout/app/messenger.gui"));

            _messages = Gui.FindById<StackPanel>("messages");

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