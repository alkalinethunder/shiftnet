using System;
using AlkalineThunder.Pandemic.Gui.Controls;

namespace Shiftnet.AppHosts
{
    public interface IShiftAppHost
    {
        string Title { get; set; }
        Desktop ShiftOS { get; }
        CanvasPanel Gui { get; }
        
        void Close();
        
        event EventHandler Closed; 
    }}