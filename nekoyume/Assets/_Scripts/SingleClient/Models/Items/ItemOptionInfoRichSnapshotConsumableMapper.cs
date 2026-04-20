#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Nekoyume.Battle;
using Nekoyume.SingleClient.Models.Stats;
using Nekoyume.SingleClient.Models.TableData;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Projects <see cref="ConsumableSnapshot"/> into <see cref="ItemOptionInfoRichSnapshot"/>
    /// matching lib9c's <c>ItemOptionInfo(ItemUsable)</c> semantics (which on consumables yields
    /// <c>StatType.NONE</c> main stat, per-stat entries with <c>count = 1</c>, no skill options,
    /// and CP = <c>CPHelper.GetCP(itemUsable)</c>). Kept in a sibling file to the equipment mapper
    /// so the UI migration sites that dispatch on <see cref="IItemSnapshot"/> (e.g.
    /// <c>CombinationResultScreen</c>) can project either subtype with symmetric calls.
    /// </summary>
    public static class ItemOptionInfoRichSnapshotConsumableMapper
    {
        public static ItemOptionInfoRichSnapshot ToItemOptionInfoRichSnapshot(
            this ConsumableSnapshot snapshot)
        {
            var stats = snapshot.Stats ?? System.Array.Empty<StatView>();
            var statOptions = new List<(StatType type, long value, int count)>(stats.Count);
            decimal cpDecimal = 0m;
            foreach (var stat in stats)
            {
                // Total value is the entire magnitude for consumables (base + additional).
                var totalValue = (long)stat.TotalValue;
                statOptions.Add((stat.StatType, totalValue, 1));
                cpDecimal += CPHelper.GetStatCP(
                    (Lib9cStatType)(int)stat.StatType,
                    stat.TotalValue);
            }

            // No skill multiplier — consumables do not route through Skills in the CP formula.
            var cp = CPHelper.DecimalToLong(cpDecimal);

            return new ItemOptionInfoRichSnapshot(
                optionCountFromCombination: stats.Count,
                mainStat: (StatType.NONE, 0, 0),
                statOptions: statOptions,
                skillOptions: System.Array.Empty<(SkillSheetRowView skillRow, long power, int chance, int statPowerRatio, StatType refStatType)>(),
                cp: cp);
        }
    }
}

#endif
