#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Lib9cItemSubType = Nekoyume.Model.Item.ItemSubType;
using Lib9cItemType = Nekoyume.Model.Item.ItemType;
using Lib9cLockType = Nekoyume.Model.Item.LockType;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Ordinal-compatible casts between lib9c <c>ItemType</c>/<c>ItemSubType</c>/<c>LockType</c>
    /// and their client-owned mirrors. Explicit helpers so boundary crossings show up in diffs
    /// instead of hiding behind implicit casts.
    /// </summary>
    public static class ItemEnumMapper
    {
        public static ItemType ToView(this Lib9cItemType source) => (ItemType)(int)source;

        public static Lib9cItemType ToLib9c(this ItemType source) => (Lib9cItemType)(int)source;

        public static ItemSubType ToView(this Lib9cItemSubType source) =>
            (ItemSubType)(int)source;

        public static Lib9cItemSubType ToLib9c(this ItemSubType source) =>
            (Lib9cItemSubType)(int)source;

        public static LockType ToView(this Lib9cLockType source) => (LockType)(int)source;

        public static Lib9cLockType ToLib9c(this LockType source) => (Lib9cLockType)(int)source;
    }
}

#endif
