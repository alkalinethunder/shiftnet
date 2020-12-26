using Shiftnet.Data;

namespace Shiftnet.Modules
{
    public class CodeShopUpgrade
    {
        public string Id { get; }
        public string Description { get; }
        public string Name { get; }
        public string Category { get; }

        public CodeShopUpgrade(CodeShopUpgradeList listInfo, CodeShopUpgradeInfo upgrade)
        {
            Category = listInfo.Name;
            Id = listInfo.Id + ":" + upgrade.Id;
            Name = upgrade.Name;
            Description = upgrade.Description;
        }
    }
}