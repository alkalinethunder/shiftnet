using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Plex.Engine;
using Plex.Engine.GUI;
using Plex.Objects;

namespace Shiftnet.Apps
{
    [AppInformation("Code Shop", "Upgrade your ShiftOS environment and hacking tools with new features and abilities.")]
    public class CodeShop : ShiftApp
    {
        private TextControl _mainTitle = new TextControl();
        private ListBox upgradelist = null;
        private ShiftoriumUpgrade selectedUpgrade = null;
        private ProgressBar upgradeprogress = null;
        private Button buy = null;
        private TextControl _upgradeTitle = new TextControl();
        private TextControl _upgradeDescription = new TextControl();
        private Dictionary<string, ShiftoriumUpgrade> _upgradedatabase = new Dictionary<string, ShiftoriumUpgrade>();

        public CodeShop()
        {
            _mainTitle.Text = "Upgrades";
            _mainTitle.AutoSize = true;
        }

        protected void OnLayout(GameTime gameTime)
        {
            try
            {
                upgradelist.X = 30;
                upgradelist.Y = 75;
                upgradelist.Width -= 30;
                upgradeprogress.X = upgradelist.X;
                upgradeprogress.Y = upgradelist.Y + upgradelist.Height + 10;
                upgradeprogress.Width = upgradelist.Width;
                upgradeprogress.Height = 24;
                buy.Visible = (selectedUpgrade != null);
                _upgradeTitle.FontStyle = TextControlFontStyle.Header2;
                _upgradeTitle.AutoSize = true;
                _upgradeTitle.Y = 15;
                _upgradeTitle.X = upgradelist.X + upgradelist.Width + 15;

                _upgradeDescription.X = _upgradeTitle.X;
                _upgradeDescription.Y = _upgradeTitle.Y + _upgradeTitle.Height + 10;

                _mainTitle.Y = upgradelist.Y - _mainTitle.Height - 5;
                _mainTitle.FontStyle = TextControlFontStyle.Header1;
                _mainTitle.MaxWidth = upgradelist.Width;
                _mainTitle.X = upgradelist.X + ((upgradelist.Width - _mainTitle.Width) / 2);
            }
            catch
            {

            }
        }

        public void OnLoad()
        {
            buy = new Button();
            buy.Text = "Buy upgrade";
            buy.AutoSize = true;
            buy.Click += () =>
            {
                if (Upgrades.UpgradeInstalled(selectedUpgrade.ID))
                {
                    try
                    {
                        if (Upgrades.IsLoaded(selectedUpgrade.ID))
                        {
                            Upgrades.UnloadUpgrade(selectedUpgrade.ID);
                        }
                        else
                        {
                            Upgrades.LoadUpgrade(selectedUpgrade.ID);

                        }
                        PopulateList();
                        SelectUpgrade(null);
                    }
                    catch (UpgradeException ex)
                    {
                        Plex.Engine.Infobox.Show("Upgrade error!", ex.ErrorMessage);
                    }
                }
                else
                {
                    string error = "";
                    if (Upgrades.Buy(selectedUpgrade.ID, out error) == true)
                    {
                        Plex.Engine.Infobox.Show("Upgrade installed!", "You have successfully bought and installed the " + selectedUpgrade.Name + " upgrade for " + selectedUpgrade.Cost + " Experience.");
                        SelectUpgrade(null);
                        PopulateList();
                    }
                    else
                    {
                        Plex.Engine.Infobox.Show("Cannot buy upgrade.", error);
                    }
                }
            };
            upgradelist = new ListBox();
            upgradeprogress = new ProgressBar();
            upgradelist.SelectedIndexChanged += () =>
            {
                if (upgradelist.SelectedItem != null)
                {
                    if(upgradelist.SelectedItem != null)
                    {
                        string upgstr = upgradelist.SelectedItem.ToString();
                        SelectUpgrade(_upgradedatabase[upgstr]);
                        return;
                    }
                    SelectUpgrade(null);
                }
            };
            PopulateList();
            SelectUpgrade(null);
        }

        public void SelectUpgrade(ShiftoriumUpgrade upgrade)
        {
            if(selectedUpgrade != upgrade)
            {
                selectedUpgrade = upgrade;
                if (upgrade == null)
                    return;
                if (Upgrades.UpgradeInstalled(upgrade.ID))
                {
                    string type = (Upgrades.IsLoaded(upgrade.ID)) ? "Unload" : "Load";
                    buy.Text = $"{type} upgrade";
                }
                else
                {
                    buy.Text = "Buy upgrade";
                }
            }

            string title = "Welcome to the Shiftorium!";
            string desc = @"The Shiftorium is a place where you can buy upgrades for your computer. These upgrades include hardware enhancements, kernel and software optimizations and features, new programs, upgrades to existing programs, and more.

As you continue through your job, going further up the ranks, you will unlock additional upgrades which can be found here. You may also find upgrades which are not available within the Shiftorium when hacking more difficult and experienced targets. These upgrades are very rare and hard to find, though. You'll find them in the ""Installed Upgrades"" list.";

            if (selectedUpgrade != null)
            {
                title = selectedUpgrade.Category + ": " + selectedUpgrade.Name;
                if (Upgrades.UpgradeInstalled(selectedUpgrade.ID))
                {
                    desc = (string.IsNullOrEmpty(selectedUpgrade.Tutorial)) ? "No tutorial has been provided for this upgrade." : selectedUpgrade.Tutorial;
                }
                else
                {
                    desc = selectedUpgrade.Description;
                }
            }
            _upgradeTitle.Text = title;
            _upgradeDescription.Text = desc;

        }

        private void PopulateListInternal()
        {
            _upgradedatabase.Clear();
            foreach(var upgrade in Upgrades.GetAvailableIDs())
            {
                var data = Upgrades.GetUpgradeInfo(upgrade);
                string type = "unknown";
                if (data.Purchasable)
                    type = $"${((double)data.Cost) / 100}";
                if (Upgrades.UpgradeInstalled(upgrade))
                {
                    type = (Upgrades.IsLoaded(upgrade)) ? "loaded" : "unloaded";
                }
                _upgradedatabase.Add($"{data.Category}: {data.Name} ({type})", data);
                Plex.Engine.Desktop.InvokeOnWorkerThread(() =>
                {
                    upgradelist.AddItem($"{data.Category}: {data.Name} ({type})");
                });
            }

            upgradeprogress.Maximum = _upgradedatabase.Count;
            upgradeprogress.Value = Upgrades.GetAvailableIDs().Where(x => Upgrades.UpgradeInstalled(x)).Count();

        }

        public void PopulateList()
        {
            upgradelist.ClearItems();
            Task.Run(() => PopulateListInternal());

        }

        public void OnSkinLoad()
        {
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
            PopulateList();
        }

        protected override void Main()
        {
            
        }
    }
}
