using Nekoyume.SingleClient.Models.Buffs;
using Lib9cCostumeStatSheet = Nekoyume.TableData.CostumeStatSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cCostumeStatSheet.Row"/> into the client-owned
    /// <see cref="CostumeStatSheetRowView"/>.
    /// </summary>
    public static class CostumeStatSheetRowMapper
    {
        public static CostumeStatSheetRowView ToView(this Lib9cCostumeStatSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new CostumeStatSheetRowView(
                id: source.Id,
                costumeId: source.CostumeId,
                statType: BuffViewMapper.MapStatType(source.StatType),
                stat: source.Stat);
        }
    }
}
