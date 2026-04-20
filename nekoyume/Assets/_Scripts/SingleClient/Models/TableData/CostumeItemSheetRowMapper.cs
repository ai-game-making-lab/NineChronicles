using Nekoyume.SingleClient.Models.Elemental;
using Nekoyume.SingleClient.Models.Items;
using Lib9cCostumeItemSheet = Nekoyume.TableData.CostumeItemSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cCostumeItemSheet.Row"/> into the client-owned
    /// <see cref="CostumeItemSheetRowView"/>.
    /// </summary>
    public static class CostumeItemSheetRowMapper
    {
        public static CostumeItemSheetRowView ToView(this Lib9cCostumeItemSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new CostumeItemSheetRowView(
                id: source.Id,
                itemSubType: source.ItemSubType.ToView(),
                grade: source.Grade,
                elementalType: source.ElementalType.ToView(),
                spineResourcePath: source.SpineResourcePath);
        }
    }
}
