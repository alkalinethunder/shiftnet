using System;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Shiftnet.AppHosts
{
    public sealed class StatusIconHost : IShiftAppHost
    {
        private Icon _icon;
        private CanvasPanel _gui;
        private Desktop _shiftos;
        
        public string Title { get; set; }
        public Desktop ShiftOS => _shiftos;
        public CanvasPanel Gui => _gui;

        public Texture2D Icon
        {
            get => _icon.Image;
            set => _icon.Image = value;
        }

        public StatusIconHost(Desktop shiftos, Control icon, CanvasPanel gui)
        {
            _shiftos = shiftos;
            _gui = gui;
            _icon = icon.FindById<Icon>("icon");
            
            _icon.Click += IconClick;
        }

        private void IconClick(object? sender, MouseButtonEventArgs e)
        {
            StatusIconClick?.Invoke(this, e);
        }

        public void Close()
        {
            
        }

        public event EventHandler StatusIconClick;
        public event EventHandler Closed;
    }
}