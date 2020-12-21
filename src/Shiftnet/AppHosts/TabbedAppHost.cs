using System;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Input;
using Microsoft.Xna.Framework.Graphics;
using Shiftnet.Controls;

namespace Shiftnet.AppHosts
{
    public class TabbedAppHost : IShiftAppHost
    {
        private Desktop _os;
        private PanelTab _tab;
        private CanvasPanel _tabContent;

        public Texture2D Icon { get; set; }
        
        public TabbedAppHost(Desktop os, PanelTab tab, CanvasPanel contentArea)
        {
            _os = os;
            _tab = tab;
            _tabContent = contentArea;
            
            _tab.Click += TabOnClick;
            _tab.CloseRequested += TabOnCloseRequested;
        }

        private void TabOnCloseRequested(object? sender, EventArgs e)
        {
            Close();
        }

        private void TabOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (_tabContent.Parent is SwitcherPanel switcher)
            {
                switcher.ActiveItem = _tabContent;
            }
        }

        public string Title
        {
            get => _tab.Title;
            set => _tab.Title = value;
        }
        
        public Desktop ShiftOS => _os;
        public CanvasPanel Gui => _tabContent;
        public void Close()
        {
            if (_tabContent.Parent is SwitcherPanel switcher)
            {
                switcher.RemoveChild(_tabContent);
            }

            if (_tab.Parent is ContainerControl container)
            {
                container.RemoveChild(_tab);
            }

            _os.NotifyTabbedAppHostClosed(this);
            
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateActiveTab()
        {
            if (_tabContent.Parent is SwitcherPanel switcher)
            {
                _tab.IsActive = switcher.ActiveItem == _tabContent;
            }
            else
            {
                _tab.IsActive = false;
            }
        }
        
        public event EventHandler Closed;
    }
}