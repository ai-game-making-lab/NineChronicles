#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Lib9cRuneOptionSheet = Nekoyume.TableData.RuneOptionSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cRuneOptionSheet.Row"/> into the client-owned
    /// <see cref="RuneOptionSheetRowView"/>. Each nested <c>RuneOptionInfo</c> is projected via
    /// <see cref="RuneOptionInfoMapper.ToView(Lib9cRuneOptionSheet.Row.RuneOptionInfo, Nekoyume.TableData.SkillSheet.Row)"/>;
    /// the skill-row companion is not threaded here because the sheet lookup happens at the
    /// call site in the UI layer.
    /// </summary>
    public static class RuneOptionSheetRowMapper
    {
        public static RuneOptionSheetRowView ToView(this Lib9cRuneOptionSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            var map = new Dictionary<int, RuneOptionInfoView>(
                source.LevelOptionMap?.Count ?? 0);
            if (source.LevelOptionMap != null)
            {
                foreach (var pair in source.LevelOptionMap)
                {
                    map[pair.Key] = pair.Value.ToView();
                }
            }

            return new RuneOptionSheetRowView(
                runeId: source.RuneId,
                levelOptionMap: map);
        }
    }
}

#endif
