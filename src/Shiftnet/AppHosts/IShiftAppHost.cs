using System;

namespace Shiftnet.AppHosts
{
    public interface IShiftAppHost
    {
        string Title { get; set; }
        Desktop ShiftOS { get; }

        void Close();
        
        event EventHandler Closed; 
    }}