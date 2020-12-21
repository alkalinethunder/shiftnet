using System;
using System.Numerics;
using System.Text;
using AlkalineThunder.Pandemic;
using AlkalineThunder.Pandemic.Gui;
using Shiftnet.AppHosts;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Skinning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shiftnet.Apps
{
    public abstract class ShiftApp : IGuiContext
    {
        private IShiftAppHost _appHost;
        private string[] _args;
        private PropertySet _properties;
        
        protected CanvasPanel Gui
            => _appHost.Gui;
        
        protected string[] Arguments
            => _args;

        protected Desktop ShiftOS
            => _appHost.ShiftOS;

        protected PropertySet LaunchProperties
            => _properties;

        protected internal IShiftAppHost AppHost => _appHost;
        
        protected string CurrentDirectory { get; private set; }

        public Texture2D Icon
        {
            get => _appHost.Icon;
            set => _appHost.Icon = value;
        }
        
        public string Title
        {
            get => _appHost.Title;
            protected set => _appHost.Title = value;
        }
        
        public void Close()
            => _appHost.Close();

        public void Initialize(IShiftAppHost appHost, PropertySet args, string cwd)
        {
            _properties = args ?? new PropertySet();
            _args = _properties.GetValue<string[]>("Arguments", Array.Empty<string>());
            
            if (_appHost != null)
                throw new InvalidOperationException("Application has already started.");


            CurrentDirectory = cwd;
            
            _appHost = appHost ?? throw new ArgumentNullException(nameof(appHost));

            _appHost.Closed += AppHostOnClosed;
            
            try
            {
                Main();
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Unfortunately, {Title} encountered an error and couldn't be launched.");
                sb.AppendLine();
                sb.AppendLine("Error details:");
                sb.AppendLine();
#if DEBUG
                sb.Append(ex.ToString());
#else
                sb.Append(ex.Message);
#endif
                
                ShiftOS.ShowInfobox(Title + " couldn't launch.", sb.ToString());
                Close();
            }
        }

        private void AppHostOnClosed(object? sender, EventArgs e)
        {
            OnClosed(e);
        }

        protected abstract void Main();
        public SceneSystem SceneSystem => ShiftOS.SceneSystem;
        public SkinSystem Skin => ShiftOS.Skin;

        protected virtual void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }
        
        public event EventHandler Closed;
    }
}