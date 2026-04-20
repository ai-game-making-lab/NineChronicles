#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of
    /// <c>Nekoyume.TableData.EquipmentItemSubRecipeSheetV2.OptionInfo</c>. A sub-recipe option
    /// entry links the rolled option id to the ratio (out of 10000) and the required block
    /// index the combination action uses to stagger result availability.
    /// </summary>
    public readonly struct EquipmentItemSubRecipeSheetV2OptionInfoView :
        IEquatable<EquipmentItemSubRecipeSheetV2OptionInfoView>
    {
        public int Id { get; }
        public int Ratio { get; }
        public int RequiredBlockIndex { get; }

        public EquipmentItemSubRecipeSheetV2OptionInfoView(int id, int ratio, int requiredBlockIndex)
        {
            Id = id;
            Ratio = ratio;
            RequiredBlockIndex = requiredBlockIndex;
        }

        public bool Equals(EquipmentItemSubRecipeSheetV2OptionInfoView other) =>
            Id == other.Id && Ratio == other.Ratio && RequiredBlockIndex == other.RequiredBlockIndex;

        public override bool Equals(object obj) =>
            obj is EquipmentItemSubRecipeSheetV2OptionInfoView other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Id;
                hash = (hash * 397) ^ Ratio;
                hash = (hash * 397) ^ RequiredBlockIndex;
                return hash;
            }
        }

        public static bool operator ==(
            EquipmentItemSubRecipeSheetV2OptionInfoView left,
            EquipmentItemSubRecipeSheetV2OptionInfoView right) => left.Equals(right);

        public static bool operator !=(
            EquipmentItemSubRecipeSheetV2OptionInfoView left,
            EquipmentItemSubRecipeSheetV2OptionInfoView right) => !left.Equals(right);
    }
}

#endif
