using System;
using System.Collections.Generic;

namespace Shiftnet.Data
{
    [Serializable]
    public class CodeShopUpgradeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewTexturePath { get; set; }
        public string[] Requirements { get; set; }
        public int Cost { get; set; }
        public List<string> Tutorial { get; set; }
    }
}