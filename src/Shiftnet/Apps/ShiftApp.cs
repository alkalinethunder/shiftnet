using System;
using Shiftnet.AppHosts;

namespace Shiftnet.Apps
{
    public abstract class ShiftApp
    {
        private IShiftAppHost _appHost;
        private string[] _args;

        protected string[] Arguments
            => _args;

        protected Desktop ShiftOS
            => _appHost.ShiftOS;
        
        public string Title
        {
            get => _appHost.Title;
            protected set => _appHost.Title = value;
        }
        
        public void Close()
            => _appHost.Close();

        public void Initialize(IShiftAppHost appHost, string[] args)
        {
            if (_appHost != null)
                throw new InvalidOperationException("Application has already started.");

            _appHost = appHost ?? throw new ArgumentNullException(nameof(appHost));
            _args = args;
            
            Main();
        }

        protected abstract void Main();
    }
}