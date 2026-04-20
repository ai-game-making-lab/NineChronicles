#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Client mirror of the counts exposed by lib9c <c>ItemOptionInfo(Equipment)</c>. The full
    /// lib9c class carries per-option tuples (<c>StatOptions</c>, <c>SkillOptions</c>) that depend
    /// on lib9c types (<c>SkillSheet.Row</c>, <c>StatType</c>). The snapshot deliberately narrows
    /// to the two aggregate counts the inventory / tooltip / equipment-slot tag UIs actually read
    /// (<c>StatOptions.Sum(x =&gt; x.count)</c> and <c>SkillOptions.Count</c>) — deeper per-option
    /// consumers (<c>CombinationResultScreen</c>) keep the lib9c helper until a richer DTO lands.
    /// </summary>
    public readonly struct ItemOptionInfoSnapshot
    {
        /// <summary>
        /// Matches lib9c <c>ItemOptionInfo.OptionCountFromCombination</c>. Non-zero means the
        /// equipment was crafted with a combination option; zero means a vanilla/non-crafted item.
        /// </summary>
        public int OptionCountFromCombination { get; }

        /// <summary>
        /// Equivalent to <c>ItemOptionInfo.StatOptions.Sum(x =&gt; x.count)</c>. Count of the stat
        /// tag icons the option tag UI should render.
        /// </summary>
        public int StatOptionTotalCount { get; }

        /// <summary>
        /// Equivalent to <c>ItemOptionInfo.SkillOptions.Count</c>. Count of the skill tag icons.
        /// </summary>
        public int SkillOptionCount { get; }

        public ItemOptionInfoSnapshot(int optionCountFromCombination, int skillOptionCount)
        {
            OptionCountFromCombination = optionCountFromCombination;
            SkillOptionCount = skillOptionCount;
            StatOptionTotalCount = optionCountFromCombination - skillOptionCount;
            if (StatOptionTotalCount < 0)
            {
                StatOptionTotalCount = 0;
            }
        }
    }

    /// <summary>
    /// Projects <see cref="EquipmentSnapshot"/> fields into an <see cref="ItemOptionInfoSnapshot"/>
    /// matching lib9c <c>ItemOptionInfo(Equipment)</c>'s count semantics:
    /// <list type="bullet">
    ///   <item><description>When lib9c <c>optionCountFromCombination &gt; 0</c>, it's the authoritative total.</description></item>
    ///   <item><description>Otherwise the total is <c>additionalStats.Count + Skills.Count</c> (additional-only stats on the StatsMap plus attached skills).</description></item>
    /// </list>
    /// Snapshot's <see cref="EquipmentSnapshot.StatsMap"/> already carries the <c>GetDecimalStats(true)</c> projection,
    /// so "additional stats" becomes "entries whose <c>AdditionalValue &gt; 0</c>".
    /// </summary>
    public static class ItemOptionInfoSnapshotMapper
    {
        public static ItemOptionInfoSnapshot ToItemOptionInfoSnapshot(this EquipmentSnapshot snapshot)
        {
            var skillsCount = snapshot.Skills?.Count ?? 0;
            var raw = snapshot.OptionCountFromCombination ?? 0;
            if (raw > 0)
            {
                return new ItemOptionInfoSnapshot(raw, skillsCount);
            }

            var additionalStatsCount = 0;
            if (snapshot.StatsMap != null)
            {
                foreach (var stat in snapshot.StatsMap)
                {
                    if (stat.AdditionalValue > 0m)
                    {
                        additionalStatsCount++;
                    }
                }
            }

            return new ItemOptionInfoSnapshot(additionalStatsCount + skillsCount, skillsCount);
        }
    }
}

#endif
