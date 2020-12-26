using AlkalineThunder.Pandemic.Gui;
using AlkalineThunder.Pandemic.Gui.Controls;
using AlkalineThunder.Pandemic.Gui.Markup;
using Microsoft.Xna.Framework;
using Shiftnet.Modules;

namespace Shiftnet.Apps
{
    [AppInformation("Skill Display Widget", "Displays information about your skill and progress.", DisplayTarget.InfoWidgets, Startup = true, PlayerOnly = true)]
    public class SkillDisplay : ShiftApp
    {
        private TextBlock _levelName;
        private TextBlock _xp;
        private ProgressBar _levelProgress;
        private TextBlock _levelStart;
        private TextBlock _levelEnd;

        private GameplayManager GameplayManager
            => ShiftOS.GetModule<GameplayManager>();
        
        protected override void Main()
        {
            Gui.AddChild(GuiBuilder.Build(this, "layout/app/skillWidget.gui"));

            _levelName = Gui.FindById<TextBlock>("levelName");
            _levelStart = Gui.FindById<TextBlock>("levelStart");
            _levelEnd = Gui.FindById<TextBlock>("levelEnd");
            _xp = Gui.FindById<TextBlock>("xp");
            _levelProgress = Gui.FindById<ProgressBar>("levelProgress");

            GameplayManager.PlayerSkillChanged += (o, a) =>
            {
                UpdateSkill();
            };
            
            UpdateSkill();
        }

        private void UpdateSkill()
        {
            var skill = GameplayManager.GetPlayerSkill();

            _levelName.Text = skill.LevelName;
            _xp.Text = $"Level {skill.Level} / {skill.XP} XP";

            _levelStart.Text = skill.LevelStart.ToString();
            _levelEnd.Text = skill.NextLevelXP.ToString();

            var levelAmount = skill.NextLevelXP - skill.LevelStart;
            var xpInLevel = skill.XP - skill.LevelStart;

            var progress = MathHelper.Clamp((float) xpInLevel / (float) levelAmount, 0, 1);

            _levelProgress.Percentage = progress;
        }
    }
}