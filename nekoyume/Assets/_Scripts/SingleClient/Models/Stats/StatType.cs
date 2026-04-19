namespace Nekoyume.SingleClient.Models.Stats
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Stat.StatType</c>. Enum ordering is deliberately
    /// kept identical so casts between the two stay lossless while Step 4 migrates callers.
    /// </summary>
    public enum StatType
    {
        NONE,
        HP,
        ATK,
        DEF,
        CRI,
        HIT,
        SPD,
        DRV,
        DRR,
        CDMG,
        ArmorPenetration,
        Thorn,
    }
}
