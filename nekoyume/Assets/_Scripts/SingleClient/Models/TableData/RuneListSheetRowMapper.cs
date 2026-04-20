#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Lib9cRuneListSheet = Nekoyume.TableData.Rune.RuneListSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cRuneListSheet.Row"/> into the client-owned
    /// <see cref="RuneListSheetRowView"/>.
    /// </summary>
    public static class RuneListSheetRowMapper
    {
        public static RuneListSheetRowView ToView(this Lib9cRuneListSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new RuneListSheetRowView(
                id: source.Id,
                grade: source.Grade,
                runeType: source.RuneType,
                requiredLevel: source.RequiredLevel,
                usePlace: source.UsePlace,
                bonusCoef: source.BonusCoef);
        }
    }
}

#endif
