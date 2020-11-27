using AlkalineThunder.Pandemic;

namespace Shiftnet.Shiftorium
{
    public class Upload
    {
        public Upload(PropertySet json)
        {
            
        }
        
        public string Slug { get; }
        public string FileName { get; }
        public long FileSize { get; }
    }
}