namespace Nekoyume.SingleClient.Models.EnumType
{
    /// <summary>
    /// Client-owned mirror of <c>Nekoyume.Model.EnumType.ArenaType</c>. Enum ordering and
    /// membership matches lib9c 1:1 so ordinal-compatible <c>(int)</c> casts stay lossless
    /// while UI call sites migrate off the lib9c namespace.
    /// </summary>
    public enum ArenaType
    {
        OffSeason,
        Season,
        Championship,
    }
}
