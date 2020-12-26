using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using AlkalineThunder.Pandemic.Input;
using Microsoft.Xna.Framework;
using Shiftnet.Data;
using Shiftnet.Modules;

namespace Shiftnet.Apps
{
    [AppInformation("Code Shop", "Upgrade your ShiftOS environment and hacking tools with new features and abilities.", Command = "upgrades", PlayerOnly = true)]
    public class CodeShop : ShiftApp
    {
        private Func<Control> MakeUpgradeButton;

        private AdvancedStackPanel _upgradeInfo;
        private TextBlock _catName;
        private TextBlock _catDesc;
        private TextBlock _catProgressText;
        private ProgressBar _catProgress;
        private TextBlock _upgName;
        private TextBlock _upgDesc;
        private StackPanel _upgrades;
        private FlowPanel _categories;
        private CodeShopCategory _category;
        private CodeShopUpgrade _upgrade;
        
        private GameplayManager GameplayManager
            => ShiftOS.GetModule<GameplayManager>();
        
        protected override void Main()
        {
            Title = "Code Shop";

            MakeUpgradeButton = GuiBuilder.MakeBuilderFunction(this, "layout/component/codeshopupgrade.gui");
            
            Gui.AddChild(GuiBuilder.Build(this, "layout/app/codeshop.gui"));

            _upgradeInfo = Gui.FindById<AdvancedStackPanel>("upgradeInfo");

            _catProgress = Gui.FindById<ProgressBar>("categoryProgress");
            _catProgressText = Gui.FindById<TextBlock>("categoryProgressText");
            
            _catName = Gui.FindById<TextBlock>("categoryName");
             _catDesc = Gui.FindById<TextBlock>("categoryDescription");

             _upgName = Gui.FindById<TextBlock>("upgradeName");
             _upgDesc = Gui.FindById<TextBlock>("upgradeDescription");

             _categories = Gui.FindById<FlowPanel>("categories");
            _upgrades = Gui.FindById<StackPanel>("upgrades");

            AddCategoryButtons();
            UpdateUpgradeList();

            GameplayManager.UpgradeUnlocked += (o, a) =>
            {
                UpdateUpgradeList();
            };
        }

        private void AddCategoryButtons()
        {
            _category = GameplayManager.GetCodeShopCategories.First();
            
            foreach (var category in GameplayManager.GetCodeShopCategories)
            {
                var button = new Button();

                button.Tag = category;
                
                var text = new TextBlock();
                text.Text = category.Name;
                button.Content = text;

                button.Click += CategoryOnClick;
                
                _categories.AddChild(button);
            }
        }

        private void UpdateUpgradeList()
        {
            // first order of business: update the active category button.
            foreach (var child in _categories.Children)
            {
                if (child is Button button && button.Tag is CodeShopCategory cat)
                {
                    button.IsActive = cat.Id == _category.Id;
                }
            }
            
            // then, deselect the current upgrade.
            _upgrade = null;
            UpdateUpgradeGui();
                
            // next, clear the upgrade list.
            _upgrades.Clear();
            
            // and now, start adding upgrades to the list.
            foreach (var upgrade in GameplayManager.GetUpgradesInCategory(_category))
            {
                var upgradeButton = MakeUpgradeButton();

                upgradeButton.Tag = upgrade;

                upgradeButton.FindById<TextBlock>("name").Text = upgrade.Name;
                upgradeButton.FindById<TextBlock>("description").Text = upgrade.Description;
                
                upgradeButton.Click += UpgradeOnClick;
                
                _upgrades.AddChild(upgradeButton);
            }
            
            // and now we set metadata.
            _catName.Text = _category.Name;
            _catDesc.Text = _category.Description;
            
            // And the progress indicator now.
            var (unlocked, total) = GameplayManager.GetUpgradeProgress(_category);

            _catProgress.Percentage = (float) unlocked / (float) total;
            _catProgressText.Text = $"{unlocked}/{total} ({Math.Round(_catProgress.Percentage * 100)}%)";
        }

        private void UpdateUpgradeGui()
        {
            // first, set the active button.
            foreach (var upgradeButton in _upgrades.Children)
            {
                if (upgradeButton is Button button && button.Tag is CodeShopUpgrade upgrade)
                {
                    button.IsActive = (_upgrade != null) && (_upgrade.Id == upgrade.Id);
                }
            }
            
            // Next, should we display the upgrade info GUI?
            _upgradeInfo.Visible = _upgrade != null;

            // Now we do the cool shit.
            if (_upgrade != null)
            {
                _upgName.Text = _upgrade.Name;
                _upgDesc.Text = _upgrade.Description;
            }
        }
        
        private void UpgradeOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && button.Tag is CodeShopUpgrade upgrade)
            {
                _upgrade = upgrade;
                UpdateUpgradeGui();
            }
        }

        private void CategoryOnClick(object? sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is CodeShopCategory cat)
                {
                    _category = cat;
                    UpdateUpgradeList();
                }
            }
        }
    }
}
