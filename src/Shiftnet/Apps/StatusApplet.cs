using System;
using System.Runtime.CompilerServices;
using Shiftnet.AppHosts;

namespace Shiftnet.Apps
{
    public abstract class StatusApplet : ShiftApp
    {
        protected StatusIconHost StatusHost => AppHost as StatusIconHost;

        protected override void Main()
        {
            if (StatusHost != null)
            {
                StatusHost.StatusIconClick += StatusHostOnStatusIconClick;
            }
        }

        private void StatusHostOnStatusIconClick(object? sender, EventArgs e)
        {
            OnClick();
        }
        
        protected virtual void OnClick() {}
    }
}