using Shiftnet.Data;

namespace Shiftnet.Modules
{
    public class CodeShopCategory
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }

        public CodeShopCategory(CodeShopUpgradeList list)
        {
            Id = list.Id;
            Name = list.Name;
            Description = list.Description;
        }
    }
}