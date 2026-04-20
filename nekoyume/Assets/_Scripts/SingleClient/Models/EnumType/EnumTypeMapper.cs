#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Lib9cArenaType = Nekoyume.Model.EnumType.ArenaType;
using Lib9cBattleType = Nekoyume.Model.EnumType.BattleType;
using Lib9cCraftType = Nekoyume.Model.EnumType.CraftType;
using Lib9cGrade = Nekoyume.Model.EnumType.Grade;
using Lib9cRuneSlotType = Nekoyume.Model.EnumType.RuneSlotType;
using Lib9cRuneType = Nekoyume.Model.EnumType.RuneType;
using Lib9cRuneUsePlace = Nekoyume.Model.EnumType.RuneUsePlace;
using Lib9cStatReferenceType = Nekoyume.Model.EnumType.StatReferenceType;
using Lib9cTradeType = Nekoyume.Model.EnumType.TradeType;

namespace Nekoyume.SingleClient.Models.EnumType
{
    /// <summary>
    /// Ordinal-compatible casts between lib9c <c>Nekoyume.Model.EnumType.*</c> enums and their
    /// client-owned mirrors. Explicit helpers so boundary crossings show up in diffs instead of
    /// hiding behind implicit casts. Follows the <c>ItemEnumMapper</c> style for consistency.
    /// </summary>
    public static class EnumTypeMapper
    {
        public static ArenaType ToView(this Lib9cArenaType source) =>
            (ArenaType)(int)source;

        public static Lib9cArenaType ToLib9c(this ArenaType source) =>
            (Lib9cArenaType)(int)source;

        public static BattleType ToView(this Lib9cBattleType source) =>
            (BattleType)(int)source;

        public static Lib9cBattleType ToLib9c(this BattleType source) =>
            (Lib9cBattleType)(int)source;

        public static CraftType ToView(this Lib9cCraftType source) =>
            (CraftType)(int)source;

        public static Lib9cCraftType ToLib9c(this CraftType source) =>
            (Lib9cCraftType)(int)source;

        public static Grade ToView(this Lib9cGrade source) =>
            (Grade)(int)source;

        public static Lib9cGrade ToLib9c(this Grade source) =>
            (Lib9cGrade)(int)source;

        public static RuneSlotType ToView(this Lib9cRuneSlotType source) =>
            (RuneSlotType)(int)source;

        public static Lib9cRuneSlotType ToLib9c(this RuneSlotType source) =>
            (Lib9cRuneSlotType)(int)source;

        public static RuneType ToView(this Lib9cRuneType source) =>
            (RuneType)(int)source;

        public static Lib9cRuneType ToLib9c(this RuneType source) =>
            (Lib9cRuneType)(int)source;

        public static RuneUsePlace ToView(this Lib9cRuneUsePlace source) =>
            (RuneUsePlace)(int)source;

        public static Lib9cRuneUsePlace ToLib9c(this RuneUsePlace source) =>
            (Lib9cRuneUsePlace)(int)source;

        public static StatReferenceType ToView(this Lib9cStatReferenceType source) =>
            (StatReferenceType)(int)source;

        public static Lib9cStatReferenceType ToLib9c(this StatReferenceType source) =>
            (Lib9cStatReferenceType)(int)source;

        public static TradeType ToView(this Lib9cTradeType source) =>
            (TradeType)(int)source;

        public static Lib9cTradeType ToLib9c(this TradeType source) =>
            (Lib9cTradeType)(int)source;
    }
}

#endif
