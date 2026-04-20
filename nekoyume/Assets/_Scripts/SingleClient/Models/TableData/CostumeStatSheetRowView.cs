#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.CostumeStatSheet.Row</c>. Each row
    /// pairs a costume id with a flat stat bonus (type + magnitude). The magnitude stays
    /// <see cref="decimal"/> to match lib9c's csv schema — consumers that need integer display
    /// can cast at the call site.
    /// </summary>
    public readonly struct CostumeStatSheetRowView : IEquatable<CostumeStatSheetRowView>
    {
        public int Id { get; }
        public int CostumeId { get; }
        public StatType StatType { get; }
        public decimal Stat { get; }

        public CostumeStatSheetRowView(int id, int costumeId, StatType statType, decimal stat)
        {
            Id = id;
            CostumeId = costumeId;
            StatType = statType;
            Stat = stat;
        }

        public bool Equals(CostumeStatSheetRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is CostumeStatSheetRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(CostumeStatSheetRowView left, CostumeStatSheetRowView right) =>
            left.Equals(right);

        public static bool operator !=(CostumeStatSheetRowView left, CostumeStatSheetRowView right) =>
            !left.Equals(right);
    }
}

#endif
