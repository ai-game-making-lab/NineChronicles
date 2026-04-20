#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.Models.Stats;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.EquipmentItemSheet.Row</c>. Carries
    /// the base <c>ItemSheet.Row</c> identity fields (<see cref="Id"/>, <see cref="ItemSubType"/>,
    /// <see cref="Grade"/>, <see cref="ElementalType"/>) plus the equipment-specific columns UI
    /// tooltips read: set id, main stat, attack range, spine resource path, and optional per-
    /// level exp. The main stat is projected into <see cref="StatView"/> so consumers never have
    /// to import <c>Nekoyume.Model.Stat.DecimalStat</c>.
    /// </summary>
    public readonly struct EquipmentItemSheetRowView : IEquatable<EquipmentItemSheetRowView>
    {
        public int Id { get; }
        public ItemSubType ItemSubType { get; }
        public int Grade { get; }
        public ElementalType ElementalType { get; }
        public int SetId { get; }
        public StatView Stat { get; }
        public decimal AttackRange { get; }
        public string SpineResourcePath { get; }
        public long? Exp { get; }

        public EquipmentItemSheetRowView(
            int id,
            ItemSubType itemSubType,
            int grade,
            ElementalType elementalType,
            int setId,
            StatView stat,
            decimal attackRange,
            string spineResourcePath,
            long? exp)
        {
            Id = id;
            ItemSubType = itemSubType;
            Grade = grade;
            ElementalType = elementalType;
            SetId = setId;
            Stat = stat;
            AttackRange = attackRange;
            SpineResourcePath = spineResourcePath;
            Exp = exp;
        }

        public bool Equals(EquipmentItemSheetRowView other) => Id == other.Id;

        public override bool Equals(object obj) => obj is EquipmentItemSheetRowView other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(EquipmentItemSheetRowView left, EquipmentItemSheetRowView right) =>
            left.Equals(right);

        public static bool operator !=(EquipmentItemSheetRowView left, EquipmentItemSheetRowView right) =>
            !left.Equals(right);
    }
}

#endif
