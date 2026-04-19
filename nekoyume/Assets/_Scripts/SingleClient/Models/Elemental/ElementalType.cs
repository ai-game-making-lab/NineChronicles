namespace Nekoyume.SingleClient.Models.Elemental
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.Elemental.ElementalType</c>. Enum ordering is
    /// deliberately kept identical so casts between the two stay lossless while Step 4 migrates
    /// callers off lib9c.
    /// </summary>
    public enum ElementalType
    {
        Normal = 0,
        Fire = 1,
        Water = 2,
        Land = 3,
        Wind = 4,
    }

    public enum ElementalResult
    {
        Win,
        Draw,
        Lose,
    }
}
