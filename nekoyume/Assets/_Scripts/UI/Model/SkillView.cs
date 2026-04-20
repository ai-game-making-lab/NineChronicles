#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.L10n;
using Nekoyume.SingleClient.Models.Skills;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class SkillView : IDisposable
    {
        /// <summary>
        /// Client-owned snapshot of the skill. Drives all display strings below and is the
        /// single public handle callers forward into <c>SkillPositionTooltip.Show(SkillSnapshot)</c>.
        /// </summary>
        public readonly SkillSnapshot Snapshot;

        public readonly ReactiveProperty<string> Name = new();
        public readonly ReactiveProperty<string> Power = new();
        public readonly ReactiveProperty<string> Chance = new();

        /// <summary>
        /// Constructs a skill view from an already-projected snapshot plus the formatted power
        /// text. Callers that still hold a lib9c <c>Skill</c> project it at the boundary via
        /// <c>.ToSnapshot()</c> and compute the power string via
        /// <c>Nekoyume.SkillExtensions.EffectToString(this Skill)</c> — that helper stays on
        /// lib9c for this slice because it reads <c>SkillBuffSheet</c> / <c>StatBuffSheet</c>.
        /// </summary>
        public SkillView(SkillSnapshot snapshot, string effectText)
        {
            Snapshot = snapshot;

            Name.Value = L10nManager.Localize($"SKILL_NAME_{Snapshot.Id}");
            Chance.Value = $"{L10nManager.Localize("UI_SKILL_CHANCE")}: {Snapshot.Chance}%";

            var powerKey = Snapshot.IsBuffSkill ? "UI_SKILL_EFFECT" : "UI_SKILL_POWER";
            Power.Value = $"{L10nManager.Localize(powerKey)}: {effectText}";
        }

        public void Dispose()
        {
            Name.Dispose();
            Power.Dispose();
            Chance.Dispose();
        }
    }
}

#endif
