using System;

namespace Shiftnet.Apps
{
    [AppInformation("Contacts", "Communicate with your friends and contacts.", DisplayTarget.Feed, UserCloseable = false, PlayerOnly = true, Startup = true)]
    public class Contacts : ShiftApp
    {
        protected override void Main()
        {
            Title = "Contacts";
        }
    }
}