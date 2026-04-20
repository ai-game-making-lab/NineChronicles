using System.Linq;
using Nekoyume.Battle;
using Nekoyume.TableData;
using Lib9cStatType = Nekoyume.Model.Stat.StatType;

namespace Nekoyume.SingleClient.Models.Items
{
    /// <summary>
    /// Client-surface overloads of <see cref="CPHelper.GetCP(Nekoyume.Model.Item.ItemUsable)"/>
    /// and <see cref="CPHelper.GetCP(Nekoyume.Model.Item.Costume, CostumeStatSheet)"/>. Callers
    /// that already hold an <see cref="EquipmentSnapshot"/> or <see cref="CostumeSnapshot"/> —
    /// notably <c>UI/Module/Inventory.cs</c>, <c>CollectionInventory.cs</c>, and the Synthesis /
    /// EnhancementInventory stacks — can compute CP without casting back to lib9c.
    /// </summary>
    public static class CPHelperSnapshotExtensions
    {
        /// <summary>
        /// Mirrors <c>CPHelper.GetCP(ItemUsable)</c>. Sums per-stat CP from <see cref="EquipmentSnapshot.StatsMap"/>
        /// and multiplies by the skills multiplier computed from Skills + BuffSkills count. All
        /// per-stat formulas delegate to the existing static helpers on <see cref="CPHelper"/> so
        /// the two overloads stay in lock-step with lib9c balance changes.
        /// </summary>
        public static long GetCP(this EquipmentSnapshot snapshot)
        {
            decimal statsCp = 0m;
            foreach (var stat in snapshot.StatsMap)
            {
                statsCp += CPHelper.GetStatCP(
                    (Lib9cStatType)(int)stat.StatType,
                    stat.TotalValue);
            }

            var skillsCount = snapshot.Skills.Count + snapshot.BuffSkills.Count;
            return CPHelper.DecimalToLong(statsCp * CPHelper.GetSkillsMultiplier(skillsCount));
        }

        /// <summary>
        /// Mirrors <c>CPHelper.GetCP(Costume, CostumeStatSheet)</c>. Sums per-stat CP off the
        /// sheet rows whose <c>CostumeId</c> matches <see cref="CostumeSnapshot.Id"/>.
        /// </summary>
        public static long GetCP(this CostumeSnapshot snapshot, CostumeStatSheet sheet)
        {
            decimal cp = 0m;
            foreach (var row in sheet.OrderedList.Where(r => r.CostumeId == snapshot.Id))
            {
                cp += CPHelper.GetStatCP(row.StatType, row.Stat);
            }

            return CPHelper.DecimalToLong(cp);
        }
    }
}
