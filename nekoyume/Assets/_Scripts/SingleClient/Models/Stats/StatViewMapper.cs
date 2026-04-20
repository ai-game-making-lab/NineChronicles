#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Buffs;
using Lib9cDecimalStat = Nekoyume.Model.Stat.DecimalStat;
using Lib9cStatMap = Nekoyume.Model.Stat.StatMap;
using Lib9cStatsMap = Nekoyume.Model.Stat.StatsMap;

namespace Nekoyume.SingleClient.Models.Stats
{
    /// <summary>
    /// Extension helpers that project lib9c <see cref="Lib9cDecimalStat"/> / <see cref="Lib9cStatMap"/>
    /// / <see cref="Lib9cStatsMap"/> into the client-owned <see cref="StatView"/> DTO. Kept
    /// extension-style so each migrating call site only adds a trailing <c>.ToView()</c>.
    /// </summary>
    public static class StatViewMapper
    {
        public static StatView ToView(this Lib9cDecimalStat source)
        {
            if (source is null)
            {
                return default;
            }

            return new StatView(
                statType: BuffViewMapper.MapStatType(source.StatType),
                baseValue: source.BaseValue,
                additionalValue: source.AdditionalValue);
        }

        /// <summary>
        /// Projects a <see cref="Lib9cStatMap"/> into an ordered list of <see cref="StatView"/>,
        /// matching lib9c's <c>GetDecimalStats(true)</c> semantics (filters stats with no base or
        /// additional magnitude).
        /// </summary>
        public static IReadOnlyList<StatView> ToView(this Lib9cStatMap source)
        {
            if (source is null)
            {
                return System.Array.Empty<StatView>();
            }

            var result = new List<StatView>();
            foreach (var stat in source.GetDecimalStats(true))
            {
                result.Add(stat.ToView());
            }

            return result;
        }

        /// <summary>
        /// Projects a <see cref="Lib9cStatsMap"/> (the outer wrapper lib9c items hold) into an
        /// ordered list of <see cref="StatView"/>. Mirrors <c>StatsMap.GetDecimalStats(true)</c>.
        /// </summary>
        public static IReadOnlyList<StatView> ToView(this Lib9cStatsMap source)
        {
            if (source is null)
            {
                return System.Array.Empty<StatView>();
            }

            var result = new List<StatView>();
            foreach (var stat in source.GetDecimalStats(true))
            {
                result.Add(stat.ToView());
            }

            return result;
        }
    }
}

#endif
