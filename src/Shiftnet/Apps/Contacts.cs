using System;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using Shiftnet.Modules;

namespace Shiftnet.Apps
{
    [AppInformation("Contacts", "Communicate with your friends and contacts.", DisplayTarget.Feed, UserCloseable = false, PlayerOnly = true, Startup = true)]
    public class Contacts : ShiftApp
    {
        private Messenger _messenger;
        
        protected override void Main()
        {
            Title = "Contacts";

            Gui.AddChild(GuiBuilder.Build(this, "layout/app/contacts.gui"));
            
            Gui.FindById<Button>("testContact").Click += OnClick;
        }

        private void OnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_messenger == null)
            {
                _messenger = ShiftOS.GetModule<AppsModule>().GetLauncher<Messenger>()
                    .Launch(null, ShiftOS, CurrentDirectory) as Messenger;

                _messenger.Closed += (o, a) =>
                {
                    _messenger = null;
                };
            }
        }
    }
}