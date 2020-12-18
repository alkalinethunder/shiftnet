using System;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using Shiftnet.Modules;
using Shiftnet.Saves;

namespace Shiftnet.Apps
{
    [AppInformation("Contacts", "Communicate with your friends and contacts.", DisplayTarget.Feed, UserCloseable = false, PlayerOnly = true, Startup = true)]
    public class Contacts : ShiftApp
    {
        private Func<Control> MakeButton;
        
        private Messenger _messenger;
        private StackPanel _contactsList;
        
        private GameplayManager GameplayManager
            => ShiftOS.GetModule<GameplayManager>();
        
        protected override void Main()
        {
            MakeButton = GuiBuilder.MakeBuilderFunction(this, "layout/component/contactButton.gui");
            Title = "Contacts";

            Gui.AddChild(GuiBuilder.Build(this, "layout/app/contacts.gui"));

            _contactsList = Gui.FindById<StackPanel>("contactsList");
            
            UpdateContactsList();
            
            GameplayManager.ContactAdded += (o, a) =>
            {
                UpdateContactsList();
            };
        }

        private void UpdateContactsList()
        {
            _contactsList.Clear();

            foreach (var contact in GameplayManager.Contacts)
            {
                var button = MakeButton();

                button.FindById<TextBlock>("fullname").Text = contact.FullName;
                button.FindById<TextBlock>("username").Text = contact.Username;

                button.Tag = contact;
                
                button.Click += OnContactClick;
                
                _contactsList.AddChild(button);
            } 
        }

        private void OnContactClick(object? sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is ContactInformation contact)
                {
                    var props = new PropertySet();
                    props.SetValue("Contact", contact);

                    var launcher = ShiftOS.GetModule<AppsModule>().GetLauncher<Messenger>();
                    launcher.Launch(props, ShiftOS, CurrentDirectory);
                }
            }
        }
    }
}