using Microsoft.Xna.Framework.Graphics;
using Shiftnet.Modules;

namespace Shiftnet.Apps
{
    [AppInformation("System Status Applet: Do Not Disturb", "Provides ShiftOS's desktop with a Do Not Disturb toggle.",
        DisplayTarget.StatusIcon, Startup = true, PlayerOnly = true)]
    public class DndApplet : StatusApplet
    {
        private Texture2D _on;
        private Texture2D _off;

        private GameplayManager GameplayManager
            => ShiftOS.GetModule<GameplayManager>();
        
        protected override void Main()
        {
            _on = GameplayManager.App.Content.Load<Texture2D>("textures/do-not-disturb");
            _off = GameplayManager.App.Content.Load<Texture2D>("textures/do-not-disturb-off");
            
            
            GameplayManager.DoNotDisturbChanged += (o, a) =>
            {
                UpdateIcon();
            };
            
            UpdateIcon();
            
            base.Main();
        }

        protected override void OnClick()
        {
            GameplayManager.DoNotDisturb = !GameplayManager.DoNotDisturb;
        }

        private void UpdateIcon()
        {
            Icon = GameplayManager.DoNotDisturb ? _on : _off;
        }
    }
}