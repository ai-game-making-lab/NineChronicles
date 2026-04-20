#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.TableData;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Client-surface validators for <see cref="InfiniteTowerFloorSheet.Row"/> restrictions.
    /// Mirrors lib9c's generic <c>ValidateItemTypeRestrictions</c>/<c>ValidateItemGradeRestrictions</c>/
    /// <c>ValidateItemLevelRestrictions</c> methods but accepts an <see cref="EquipmentSnapshot"/>
    /// or <see cref="CostumeSnapshot"/> so Inventory UI dim logic can stop holding lib9c-typed
    /// <c>List&lt;Equipment&gt;</c> / <c>List&lt;Costume&gt;</c> instances. Throws the same
    /// <see cref="Exception"/> messages as the lib9c originals so <c>catch { Dim = true }</c>
    /// sites behave identically.
    /// </summary>
    public static class InfiniteTowerFloorSheetRowSnapshotExtensions
    {
        public static void ValidateEquipmentRestrictions(
            this InfiniteTowerFloorSheet.Row floor,
            EquipmentSnapshot snapshot)
        {
            ValidateSubType(floor, (Nekoyume.Model.Item.ItemSubType)(int)snapshot.ItemSubType);
            ValidateGrade(floor, snapshot.Grade);

            if (!floor.MinItemLevel.HasValue && !floor.MaxItemLevel.HasValue)
            {
                return;
            }

            var level = snapshot.Level;
            if (floor.MinItemLevel.HasValue && level < floor.MinItemLevel.Value)
            {
                throw new Exception(
                    $"Invalid item level. Item level '{level}' is below minimum requirement. " +
                    $"Minimum level required: {floor.MinItemLevel.Value}");
            }

            if (floor.MaxItemLevel.HasValue && level > floor.MaxItemLevel.Value)
            {
                throw new Exception(
                    $"Invalid item level. Item level '{level}' exceeds maximum limit. " +
                    $"Maximum level allowed: {floor.MaxItemLevel.Value}");
            }
        }

        public static void ValidateCostumeRestrictions(
            this InfiniteTowerFloorSheet.Row floor,
            CostumeSnapshot snapshot)
        {
            ValidateSubType(floor, (Nekoyume.Model.Item.ItemSubType)(int)snapshot.ItemSubType);
            ValidateGrade(floor, snapshot.Grade);
            // Costumes have no level axis — lib9c ValidateItemLevelRestrictions is a no-op for them.
        }

        private static void ValidateSubType(
            InfiniteTowerFloorSheet.Row floor,
            Nekoyume.Model.Item.ItemSubType lib9cSubType)
        {
            if (floor.ForbiddenItemSubTypes.Count == 0)
            {
                return;
            }

            if (floor.ForbiddenItemSubTypes.Contains(lib9cSubType))
            {
                throw new Exception(
                    $"Invalid item sub-type. Item sub-type '{lib9cSubType}' is forbidden. " +
                    $"Forbidden sub-types: {string.Join(", ", floor.ForbiddenItemSubTypes)}");
            }
        }

        private static void ValidateGrade(InfiniteTowerFloorSheet.Row floor, int grade)
        {
            if (!floor.MinItemGrade.HasValue && !floor.MaxItemGrade.HasValue)
            {
                return;
            }

            if (floor.MinItemGrade.HasValue && grade < floor.MinItemGrade.Value)
            {
                throw new Exception(
                    $"Invalid item grade. Item grade '{grade}' is below minimum requirement. " +
                    $"Minimum grade required: {floor.MinItemGrade.Value}");
            }

            if (floor.MaxItemGrade.HasValue && grade > floor.MaxItemGrade.Value)
            {
                throw new Exception(
                    $"Invalid item grade. Item grade '{grade}' exceeds maximum limit. " +
                    $"Maximum grade allowed: {floor.MaxItemGrade.Value}");
            }
        }
    }
}

#endif
