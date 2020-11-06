using AlkalineThunder.Pandemic.Windowing;
using Shiftnet.AppHosts;
using System;

namespace Shiftnet
{
    public class ShiftAppWindow : Window, IShiftAppHost
    {
        private Desktop _os;

        public Desktop ShiftOS => _os;

        public ShiftAppWindow LinkToDesktop(Desktop os)
        {
            if (_os != null) throw new InvalidOperationException("ShiftOS is already well aware of this window app.");
            _os = os ?? throw new ArgumentNullException(nameof(os));
            return this;
        }
    }
}