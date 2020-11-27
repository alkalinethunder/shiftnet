using System;
using System.Collections.Generic;

namespace Shiftnet.Data
{
    [Serializable]
    public class CodeShopUpgradeList
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CodeShopUpgrade> Upgrades { get; set; }
    }
}