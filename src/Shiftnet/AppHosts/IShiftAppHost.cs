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
}