namespace Nekoyume.SingleClient.Models.EnumType
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.EnumType.BattleType</c>. The lib9c
    /// <c>BattleTypeExtensions.IsEquippableRune</c> helper is intentionally NOT ported; UI
    /// callers should continue to call the lib9c extension (or receive a snapshot) rather than
    /// duplicate rune-equip policy in the client layer.
    /// </summary>
    public enum BattleType
    {
        Adventure = 1,
        Arena = 2,
        Raid = 3,
        InfiniteTower = 4,
        End = 5,
    }
}
