using System.Collections.Generic;

namespace Shiftnet
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        bool FileExists(string path);

        void CreateDirectory(string directory);

        void DeleteFile(string path);
        void DeleteDirectory(string path, bool recursive);

        string ReadAllText(string path);
        void WriteAllText(string path, string text);

        IEnumerable<string> GetDirectories(string path);
        IEnumerable<string> GetFiles(string path);
    }
}