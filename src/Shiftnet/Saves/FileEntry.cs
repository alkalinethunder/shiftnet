namespace Shiftnet.Saves
{
    public class FileEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public FileType FileType { get; set; }
        public string FileReferenceId { get; set; }
    }
}