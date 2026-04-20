using System;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.Rune.RuneListSheet.Row</c>. The
    /// lib9c row is a flat record (no nested types); <see cref="RuneType"/> and
    /// <see cref="UsePlace"/> stay typed as <see cref="int"/> here to match lib9c's csv-backed
    /// storage — callers that need enum semantics cast via
    /// <c>Nekoyume.SingleClient.Models.EnumType.EnumTypeMapper</c>.
    /// </summary>
    public readonly struct RuneListSheetRowView : IEquatable<RuneListSheetRowView>
    {
        public int Id { get; }
        public int Grade { get; }
        public int RuneType { get; }
        public int RequiredLevel { get; }
        public int UsePlace { get; }
        public int BonusCoef { get; }

        public RuneListSheetRowView(
            int id,
            int grade,
            int runeType,
            int requiredLevel,
            int usePlace,
            int bonusCoef)
        {
            Id = id;
            Grade = grade;
            RuneType = runeType;
            RequiredLevel = requiredLevel;
            UsePlace = usePlace;
            BonusCoef = bonusCoef;
        }

        public bool Equals(RuneListSheetRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is RuneListSheetRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(RuneListSheetRowView left, RuneListSheetRowView right) =>
            left.Equals(right);

        public static bool operator !=(RuneListSheetRowView left, RuneListSheetRowView right) =>
            !left.Equals(right);
    }
}
