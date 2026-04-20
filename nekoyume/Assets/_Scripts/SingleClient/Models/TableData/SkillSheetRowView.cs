#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.Helper;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Skills;
using UnityEngine;
using Lib9cSkillSheet = Nekoyume.TableData.SkillSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Client-owned immutable mirror of <see cref="Lib9cSkillSheet.Row"/>. Captures only the
    /// fields the UI tooltip/row consumers read (Id, elemental, skill type/category/target,
    /// hit count, cooldown). The underlying lib9c row is stashed in a private field so display
    /// helpers can still delegate to <c>SkillExtensions.GetLocalizedName</c> /
    /// <c>SkillIconHelper.GetSkillIcon</c> during the intermediate migration step — a later
    /// slice will drop that source reference once L10n/Resources access is ported.
    /// </summary>
    public readonly struct SkillSheetRowView : IEquatable<SkillSheetRowView>
    {
        // Source row is retained ONLY to forward display helpers. Consumers should stay on the
        // public client-typed surface.
        private readonly Lib9cSkillSheet.Row _source;

        public int Id { get; }
        public ElementalType ElementalType { get; }
        public SkillType SkillType { get; }
        public SkillCategory SkillCategory { get; }
        public SkillTargetType SkillTargetType { get; }
        public int HitCount { get; }
        public int Cooldown { get; }

        public SkillSheetRowView(
            Lib9cSkillSheet.Row source,
            int id,
            ElementalType elementalType,
            SkillType skillType,
            SkillCategory skillCategory,
            SkillTargetType skillTargetType,
            int hitCount,
            int cooldown)
        {
            _source = source;
            Id = id;
            ElementalType = elementalType;
            SkillType = skillType;
            SkillCategory = skillCategory;
            SkillTargetType = skillTargetType;
            HitCount = hitCount;
            Cooldown = cooldown;
        }

        /// <summary>
        /// Returns the localized skill name. Delegates to the lib9c
        /// <c>SkillExtensions.GetLocalizedName</c> helper by way of the retained source row so
        /// this slice does not duplicate L10n key resolution. If the row was constructed
        /// without a source (<see langword="default"/>), falls back to the raw
        /// <c>SKILL_NAME_{Id}</c> key string.
        /// </summary>
        public string GetLocalizedName()
        {
            if (_source is null)
            {
                return $"SKILL_NAME_{Id}";
            }

            return SkillExtensions.GetLocalizedName(_source);
        }

        /// <summary>
        /// Returns the skill icon sprite via <see cref="SkillIconHelper.GetSkillIcon"/>. Does
        /// not depend on the retained source row because the helper only needs <see cref="Id"/>.
        /// </summary>
        public Sprite GetIcon()
        {
            return SkillIconHelper.GetSkillIcon(Id);
        }

        public bool IsBuffSkill =>
            SkillType == SkillType.Buff ||
            SkillType == SkillType.Debuff;

        public bool Equals(SkillSheetRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is SkillSheetRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(SkillSheetRowView left, SkillSheetRowView right) => left.Equals(right);

        public static bool operator !=(SkillSheetRowView left, SkillSheetRowView right) => !left.Equals(right);
    }
}

#endif
