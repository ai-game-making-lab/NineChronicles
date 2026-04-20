using System;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Items;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.CostumeItemSheet.Row</c>. Beyond
    /// the <c>ItemSheet.Row</c> identity columns, costumes carry only the spine resource path
    /// that the render layer loads; stat bonuses live on <c>CostumeStatSheet</c> (mirrored
    /// separately by <see cref="CostumeStatSheetRowView"/>).
    /// </summary>
    public readonly struct CostumeItemSheetRowView : IEquatable<CostumeItemSheetRowView>
    {
        public int Id { get; }
        public ItemSubType ItemSubType { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public string SpineResourcePath { get; }

        public CostumeItemSheetRowView(
            int id,
            ItemSubType itemSubType,
            int grade,
            ElementalType elementalType,
            string spineResourcePath)
        {
            Id = id;
            ItemSubType = itemSubType;
            Grade = grade;
            ElementalType = elementalType;
            SpineResourcePath = spineResourcePath;
        }

        public bool Equals(CostumeItemSheetRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is CostumeItemSheetRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(CostumeItemSheetRowView left, CostumeItemSheetRowView right) =>
            left.Equals(right);

        public static bool operator !=(CostumeItemSheetRowView left, CostumeItemSheetRowView right) =>
            !left.Equals(right);
    }
}
