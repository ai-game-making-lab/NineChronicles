using System.Collections.Generic;
using Nekoyume.SingleClient.Models.EnumType;
using Nekoyume.SingleClient.Models.Skills;
using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.SingleClient.Models.TableData;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Rich client mirror of lib9c <c>ItemOptionInfo(Equipment)</c>. Carries the full per-option
    /// tuples that <c>CombinationResultScreen</c>, <c>EnhancementResultScreen</c>, and
    /// <c>Enhancement</c> consume. See <see cref="ItemOptionInfoSnapshot"/> for the narrowed
    /// count-only variant the option-tag UI uses.
    ///
    /// Mirror of lib9c <c>Nekoyume.Helper.ItemOptionInfo</c> (Lib9c/Helper/ItemOptionHelper.cs)
    /// with these tuple shapes preserved:
    /// - <see cref="MainStat"/>: (StatType type, long baseValue, long totalValue)
    /// - <see cref="StatOptions"/>: List of (StatType type, long value, int count)
    /// - <see cref="SkillOptions"/>: List of (SkillSheetRowView skillRow, long power, int chance, int statPowerRatio, StatType refStatType)
    /// - <see cref="CP"/>: long (computed via <c>CPHelperSnapshotExtensions.GetCP</c>)
    /// </summary>
    public readonly struct ItemOptionInfoRichSnapshot
    {
        public int OptionCountFromCombination { get; }

        public (StatType type, long baseValue, long totalValue) MainStat { get; }

        public IReadOnlyList<(StatType type, long value, int count)> StatOptions { get; }

        public IReadOnlyList<(SkillSheetRowView skillRow, long power, int chance, int statPowerRatio, StatType refStatType)> SkillOptions { get; }

        public long CP { get; }

        public ItemOptionInfoRichSnapshot(
            int optionCountFromCombination,
            (StatType type, long baseValue, long totalValue) mainStat,
            IReadOnlyList<(StatType type, long value, int count)> statOptions,
            IReadOnlyList<(SkillSheetRowView skillRow, long power, int chance, int statPowerRatio, StatType refStatType)> skillOptions,
            long cp)
        {
            OptionCountFromCombination = optionCountFromCombination;
            MainStat = mainStat;
            StatOptions = statOptions ?? System.Array.Empty<(StatType, long, int)>();
            SkillOptions = skillOptions ?? System.Array.Empty<(SkillSheetRowView, long, int, int, StatType)>();
            CP = cp;
        }
    }

    /// <summary>
    /// Projects <see cref="EquipmentSnapshot"/> into <see cref="ItemOptionInfoRichSnapshot"/>
    /// matching lib9c <c>ItemOptionInfo(Equipment)</c> semantics. A <see cref="SkillSheetRowView"/>
    /// lookup delegate (or pre-resolved lookup) supplies skill row metadata so the UI side can
    /// remain lib9c-free; during the transition, callers map lib9c <c>SkillSheet</c> rows via
    /// the existing <c>SkillSheetRowMapper.ToView</c> in a one-liner.
    /// </summary>
    public static class ItemOptionInfoRichSnapshotMapper
    {
        public static ItemOptionInfoRichSnapshot ToItemOptionInfoRichSnapshot(
            this EquipmentSnapshot snapshot,
            System.Func<int, SkillSheetRowView?> skillRowLookup = null)
        {
            var uniqueStatType = snapshot.UniqueStatType;

            var baseValue = 0L;
            var totalValue = 0L;
            var additionalStats = new List<(StatType type, long value)>();
            if (snapshot.StatsMap != null)
            {
                foreach (var stat in snapshot.StatsMap)
                {
                    if (stat.StatType == uniqueStatType)
                    {
                        baseValue = (long)stat.BaseValue;
                        totalValue = (long)stat.TotalValue;
                    }

                    if (stat.AdditionalValue > 0m)
                    {
                        additionalStats.Add((stat.StatType, (long)stat.AdditionalValue));
                    }
                }
            }

            var skillsCount = snapshot.Skills?.Count ?? 0;
            var optionCountFromCombination =
                snapshot.OptionCountFromCombination.GetValueOrDefault() > 0
                    ? snapshot.OptionCountFromCombination.Value
                    : additionalStats.Count + skillsCount;

            var optionCountDiff = optionCountFromCombination - (additionalStats.Count + skillsCount);

            var statOptions = new List<(StatType type, long value, int count)>(additionalStats.Count);
            foreach (var (statType, value) in additionalStats)
            {
                if (statType == uniqueStatType && optionCountDiff > 0)
                {
                    statOptions.Add((statType, value, 1 + optionCountDiff));
                    continue;
                }

                statOptions.Add((statType, value, 1));
            }

            var skillOptions = new List<(SkillSheetRowView skillRow, long power, int chance, int statPowerRatio, StatType refStatType)>(skillsCount);
            if (snapshot.Skills != null && skillRowLookup != null)
            {
                foreach (var skill in snapshot.Skills)
                {
                    var row = skillRowLookup(skill.Id);
                    if (row is null)
                    {
                        continue;
                    }

                    skillOptions.Add((
                        row.Value,
                        skill.Power,
                        skill.Chance,
                        skill.StatPowerRatio,
                        skill.ReferencedStatType));
                }
            }

            var cp = snapshot.GetCP();

            return new ItemOptionInfoRichSnapshot(
                optionCountFromCombination,
                (uniqueStatType, baseValue, totalValue),
                statOptions,
                skillOptions,
                cp);
        }
    }
}
