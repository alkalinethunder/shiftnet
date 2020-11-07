namespace Shiftnet
{
    public interface IShiftOS
    {
        string HostName { get; }
        IFileSystem FileSystem { get; }
    }
}