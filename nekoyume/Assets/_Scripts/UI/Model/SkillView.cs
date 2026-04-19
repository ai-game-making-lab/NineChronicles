using System;
using Nekoyume.L10n;
using Nekoyume.SingleClient.Models.Skills;
using UniRx;
using Lib9cBuffSkill = Nekoyume.Model.Skill.BuffSkill;
using Lib9cSkill = Nekoyume.Model.Skill.Skill;
// Lib9cBuffSkill is kept so the second ctor overload can preserve the existing
// `new Model.SkillView(buffSkill)` call shape from ItemTooltipDetail.

namespace Nekoyume.UI.Model
{
    public class SkillView : IDisposable
    {
        /// <summary>
        /// Client-owned snapshot of the skill. Drives all display strings below.
        /// </summary>
        public readonly SkillSnapshot Snapshot;

        /// <summary>
        /// Retained lib9c reference used only to forward into
        /// <see cref="Nekoyume.UI.Module.Common.SkillPositionTooltip.Show(Lib9cSkill)"/>, which
        /// is explicitly deferred out of this migration slice because it also consumes lib9c
        /// TableData rows (SkillSheet.Row / StatBuffSheet / BuffLimitSheet). Will be dropped
        /// once the tooltip is ported to SkillSnapshot + client row mirrors.
        /// </summary>
        public readonly Lib9cSkill Skill;

        public readonly ReactiveProperty<string> Name = new();
        public readonly ReactiveProperty<string> Power = new();
        public readonly ReactiveProperty<string> Chance = new();

        public SkillView(Lib9cSkill skill)
        {
            Skill = skill;
            Snapshot = skill.ToSnapshot();

            Name.Value = L10nManager.Localize($"SKILL_NAME_{Snapshot.Id}");
            Chance.Value = $"{L10nManager.Localize("UI_SKILL_CHANCE")}: {Snapshot.Chance}%";

            // EffectToString still lives on lib9c (SkillExtensions.EffectToString(this Skill))
            // because it consumes SkillSheet + StatBuffSheet rows; once those rows have client
            // mirrors the lib9c dispatch can retire and the call can run straight from Snapshot.
            var powerValue = skill.EffectToString();
            var powerKey = Snapshot.IsBuffSkill ? "UI_SKILL_EFFECT" : "UI_SKILL_POWER";
            Power.Value = $"{L10nManager.Localize(powerKey)}: {powerValue}";
        }

        public SkillView(Lib9cBuffSkill skill)
            : this((Lib9cSkill)skill)
        {
        }

        public void Dispose()
        {
            Name.Dispose();
            Power.Dispose();
            Chance.Dispose();
        }
    }
}
