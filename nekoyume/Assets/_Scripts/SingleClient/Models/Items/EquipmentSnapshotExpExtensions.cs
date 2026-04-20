#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Linq;
using Nekoyume.TableData;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Mirrors lib9c <c>Equipment.GetRealExp(EquipmentItemSheet, EnhancementCostSheetV3)</c> for
    /// <see cref="EquipmentSnapshot"/>. Enhancement UI (<c>UI/Widget/Workshop/Enhancement.cs</c>,
    /// <c>AddHammerPopup.cs</c>) calls <c>GetRealExp</c> on inventory items to compute target exp;
    /// the client overload lets those sites stay on the DTO surface while the lib9c sheets keep
    /// their original types (UI already reads sheets via <c>TableSheets</c>).
    /// </summary>
    public static class EquipmentSnapshotExpExtensions
    {
        public static long GetRealExp(
            this EquipmentSnapshot snapshot,
            EquipmentItemSheet itemSheet,
            EnhancementCostSheetV3 costSheet)
        {
            if (snapshot.Exp != 0)
            {
                return snapshot.Exp;
            }

            if (snapshot.Level == 0)
            {
                return (long)itemSheet.OrderedList.First(r => r.Id == snapshot.Id).Exp!;
            }

            var lib9cSubType = (Nekoyume.Model.Item.ItemSubType)(int)snapshot.ItemSubType;
            return costSheet.OrderedList.First(r =>
                r.ItemSubType == lib9cSubType &&
                r.Grade == snapshot.Grade &&
                r.Level == snapshot.Level).Exp;
        }
    }
}

#endif
