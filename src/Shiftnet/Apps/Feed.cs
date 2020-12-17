namespace Shiftnet.Apps
{
    [AppInformation("Feed", "it's what's happening in the Shiftnet.", DisplayTarget.Feed, SingleInstance = true,
        PlayerOnly = true, Startup = true, UserCloseable = false)]
    public class Feed : ShiftApp
    {
        protected override void Main()
        {
            Title = "Feed";
        }
    }
}