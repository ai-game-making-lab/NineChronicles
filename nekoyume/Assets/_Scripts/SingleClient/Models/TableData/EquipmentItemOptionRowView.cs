#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.EquipmentItemOptionSheet.Row</c>.
    /// Captures the stat/skill fields equipment tooltips and the shop/grind/enhancement UI
    /// read: stat range, optional skill id + damage/chance ranges, and the stat power ratio
    /// pair. <c>StatDamageRatioMin/Max</c> from lib9c is renamed to
    /// <see cref="StatPowerRatioMin"/>/<see cref="StatPowerRatioMax"/> to stay consistent with
    /// the existing <c>SkillSnapshot</c> / <c>OptionDigest</c> field names already in use.
    /// </summary>
    public readonly struct EquipmentItemOptionRowView : IEquatable<EquipmentItemOptionRowView>
    {
        public int Id { get; }
        public StatType StatType { get; }
        public int StatMin { get; }
        public int StatMax { get; }
        public int SkillId { get; }
        public int SkillDamageMin { get; }
        public int SkillDamageMax { get; }
        public int SkillChanceMin { get; }
        public int SkillChanceMax { get; }
        public decimal StatPowerRatioMin { get; }
        public decimal StatPowerRatioMax { get; }
        public StatType ReferencedStatType { get; }

        public EquipmentItemOptionRowView(
            int id,
            StatType statType,
            int statMin,
            int statMax,
            int skillId,
            int skillDamageMin,
            int skillDamageMax,
            int skillChanceMin,
            int skillChanceMax,
            decimal statPowerRatioMin,
            decimal statPowerRatioMax,
            StatType referencedStatType)
        {
            Id = id;
            StatType = statType;
            StatMin = statMin;
            StatMax = statMax;
            SkillId = skillId;
            SkillDamageMin = skillDamageMin;
            SkillDamageMax = skillDamageMax;
            SkillChanceMin = skillChanceMin;
            SkillChanceMax = skillChanceMax;
            StatPowerRatioMin = statPowerRatioMin;
            StatPowerRatioMax = statPowerRatioMax;
            ReferencedStatType = referencedStatType;
        }

        public bool HasSkill => SkillId != 0;

        public bool Equals(EquipmentItemOptionRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is EquipmentItemOptionRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(EquipmentItemOptionRowView left, EquipmentItemOptionRowView right) =>
            left.Equals(right);

        public static bool operator !=(EquipmentItemOptionRowView left, EquipmentItemOptionRowView right) =>
            !left.Equals(right);
    }
}

#endif
