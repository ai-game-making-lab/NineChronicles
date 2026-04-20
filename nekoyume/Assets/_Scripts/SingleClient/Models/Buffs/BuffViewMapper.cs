#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Lib9cBuff = Nekoyume.Model.Buff.Buff;
using Lib9cStatBuff = Nekoyume.Model.Buff.StatBuff;
using Lib9cActionBuff = Nekoyume.Model.Buff.ActionBuff;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;
using SingleClientStatType = Nekoyume.SingleClient.Models.Stats.StatType;

namespace Nekoyume.SingleClient.Models.Buffs
{
    /// <summary>
    /// Projects the lib9c runtime <c>Buff</c> types into the client-owned <see cref="BuffView"/>
    /// DTO. Kept as an extension-style helper so Step 4 callers can opt in without breaking the
    /// existing lib9c-typed call sites.
    /// </summary>
    public static class BuffViewMapper
    {
        public static BuffView ToView(this Lib9cBuff source)
        {
            if (source is null)
            {
                return default;
            }

            var info = source.BuffInfo;
            switch (source)
            {
                case Lib9cStatBuff stat:
                    return new BuffView(
                        id: info.Id,
                        groupId: info.GroupId,
                        originalDuration: source.OriginalDuration,
                        remainedDuration: source.RemainedDuration,
                        isBuff: source.IsBuff(),
                        isDebuff: source.IsDebuff(),
                        kind: BuffViewKind.Stat,
                        statType: MapStatType(stat.RowData.StatType),
                        statValue: stat.RowData.Value);
                case Lib9cActionBuff _:
                default:
                    return new BuffView(
                        id: info.Id,
                        groupId: info.GroupId,
                        originalDuration: source.OriginalDuration,
                        remainedDuration: source.RemainedDuration,
                        isBuff: source.IsBuff(),
                        isDebuff: source.IsDebuff(),
                        kind: BuffViewKind.Action,
                        statType: SingleClientStatType.NONE,
                        statValue: 0);
            }
        }

        public static IReadOnlyDictionary<int, BuffView> ToViewMap(
            this IReadOnlyDictionary<int, Lib9cBuff> source)
        {
            var result = new Dictionary<int, BuffView>(source?.Count ?? 0);
            if (source is null)
            {
                return result;
            }

            foreach (var pair in source)
            {
                result[pair.Key] = pair.Value.ToView();
            }

            return result;
        }

        public static SingleClientStatType MapStatType(Lib9cStatType source)
        {
            // Enum members are kept ordinal-compatible with lib9c so a direct cast stays valid.
            return (SingleClientStatType)(int)source;
        }

        public static Lib9cStatType MapStatType(SingleClientStatType source)
        {
            return (Lib9cStatType)(int)source;
        }
    }
}

#endif
