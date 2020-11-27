using System;
using AlkalineThunder.Pandemic;
using System.Text.Json;

namespace Shiftnet.Shiftorium
{
    public class Skin
    {
        public Skin(JsonElement json)
        {
            
        }
        
        public string Name { get; }
        public User Author { get; }
        public DateTime Published { get; }
        public Upload Download { get; }
        public Upload Thumbnail { get; }
        public Upload[] Screenshots { get; }
    }
}