using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui.Markup;
using Microsoft.Xna.Framework;

namespace Shiftnet.Apps
{
    [AppInformation("Code Shop", "Upgrade your ShiftOS environment and hacking tools with new features and abilities.", Command = "upgrades", PlayerOnly = true)]
    public class CodeShop : ShiftApp
    {
        protected override void Main()
        {
            Title = "Code Shop";

            var gui = this.Build("layout/app/codeshop.gui");
            Gui.AddChild(gui);
        }
    }
}
