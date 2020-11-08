using System.Collections.Generic;

namespace Shiftnet
{
    public class DiskNode
    {
        public int DirectoryEntry { get; set; }
        public string Name { get; set; }
        public DiskNode Parent { get; set; }
        public List<DiskNode> Children { get; set; } = new List<DiskNode>();
        public List<DiskFileNode> Files { get; set; } = new List<DiskFileNode>();
    }

    public class DiskFileNode
    {
        public int FileEntry { get; set; }
        public string FileName { get; set; }
        public DiskNode Parent { get; set; }
    }
}