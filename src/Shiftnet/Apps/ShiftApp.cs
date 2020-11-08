using System;
using System.Text;
using AlkalineThunder.Pandemic.Gui;
using Shiftnet.AppHosts;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Scenes;
using AlkalineThunder.Pandemic.Skinning;

namespace Shiftnet.Apps
{
    public abstract class ShiftApp : IGuiContext
    {
        private IShiftAppHost _appHost;
        private string[] _args;

        protected CanvasPanel Gui
            => _appHost.Gui;
        
        protected string[] Arguments
            => _args;

        protected Desktop ShiftOS
            => _appHost.ShiftOS;
        
        protected string CurrentDirectory { get; private set; }
        
        public string Title
        {
            get => _appHost.Title;
            protected set => _appHost.Title = value;
        }
        
        public void Close()
            => _appHost.Close();

        public void Initialize(IShiftAppHost appHost, string[] args, string cwd)
        {
            if (_appHost != null)
                throw new InvalidOperationException("Application has already started.");


            CurrentDirectory = cwd;
            
            _appHost = appHost ?? throw new ArgumentNullException(nameof(appHost));
            _args = args;

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

        protected abstract void Main();
        public SceneSystem SceneSystem => ShiftOS.SceneSystem;
        public SkinSystem Skin => ShiftOS.Skin;
    }
}