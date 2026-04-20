#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Models.Buffs;
using Lib9cStatBuffSheet = Nekoyume.TableData.StatBuffSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cStatBuffSheet.Row"/> into the client-owned
    /// <see cref="StatBuffRowView"/>. Simulator-only fields (<c>TargetType</c>,
    /// <c>OperationType</c>, <c>IsEnhanceable</c>, <c>MaxStack</c>) are intentionally omitted
    /// because the UI never reads them — the skill tooltip only needs the magnitude/duration
    /// pair.
    /// </summary>
    public static class StatBuffRowMapper
    {
        public static StatBuffRowView ToView(this Lib9cStatBuffSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new StatBuffRowView(
                id: source.Id,
                groupId: source.GroupId,
                statType: BuffViewMapper.MapStatType(source.StatType),
                duration: source.Duration,
                value: source.Value,
                chance: source.Chance);
        }
    }
}

#endif
