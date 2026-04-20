#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.RuneOptionSheet.Row</c>. Each row
    /// is keyed by <see cref="RuneId"/> and exposes a per-level option map. Nested
    /// <c>RuneOptionInfo</c> entries are projected into <see cref="RuneOptionInfoView"/> so
    /// consumers stay on the client-owned surface.
    /// </summary>
    public readonly struct RuneOptionSheetRowView
    {
        public int RuneId { get; }

        /// Ordered by rune level. Missing levels are absent from the map (not <see langword="null"/>
        /// / zeroed) so callers can fall back to the nearest lower level explicitly.
        public IReadOnlyDictionary<int, RuneOptionInfoView> LevelOptionMap { get; }

        public RuneOptionSheetRowView(
            int runeId,
            IReadOnlyDictionary<int, RuneOptionInfoView> levelOptionMap)
        {
            RuneId = runeId;
            LevelOptionMap =
                levelOptionMap ?? new Dictionary<int, RuneOptionInfoView>();
        }

        /// Convenience lookup that mirrors lib9c's <c>RuneOptionSheet.TryGetOptionInfo</c> shape
        /// so UI call sites can migrate without re-wiring control flow.
        public bool TryGetOptionInfo(int level, out RuneOptionInfoView optionInfo)
        {
            if (LevelOptionMap != null && LevelOptionMap.TryGetValue(level, out var info))
            {
                optionInfo = info;
                return true;
            }

            optionInfo = default;
            return false;
        }
    }
}

#endif
