using System;
using AlkalineThunder.Pandemic.Gui.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace Shiftnet.AppHosts
{
    public interface IShiftAppHost
    {
        string Title { get; set; }
        Desktop ShiftOS { get; }
        CanvasPanel Gui { get; }
        Texture2D Icon { get; set; }
        
        void Close();
        
        event EventHandler Closed; 
    }

    public class InfoWidgetHost : IShiftAppHost
    {
        private CanvasPanel _canvas;
        private Desktop _shiftos;
        
        public string Title { get; set; }
        public Desktop ShiftOS => _shiftos;
        public CanvasPanel Gui => _canvas;
        public Texture2D Icon { get; set; }

        public InfoWidgetHost(Desktop shiftos, CanvasPanel canvas)
        {
            _shiftos = shiftos;
            _canvas = canvas;
        }
        
        public void Close()
        {
            if (_canvas.Parent is StackPanel stacker)
            {
                stacker.RemoveChild(_canvas);
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Closed;
    }
}