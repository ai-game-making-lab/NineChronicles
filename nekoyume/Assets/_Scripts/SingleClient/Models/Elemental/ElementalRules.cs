using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.Elemental
{
    /// <summary>
    /// Deterministic elemental rock-paper-scissors rules ported from
    /// <c>Nekoyume.Model.Elemental.ElementalTypeExtension</c> so UI callers can evaluate
    /// multipliers and damage without dragging in lib9c.
    /// </summary>
    public static class ElementalRules
    {
        public const decimal WinMultiplier = 1.2m;

        private static List<ElementalType> _allTypes;

        public static bool TryGetWinCase(this ElementalType win, out ElementalType lose)
        {
            switch (win)
            {
                case ElementalType.Normal:
                    lose = ElementalType.Normal;
                    return false;
                case ElementalType.Fire:
                    lose = ElementalType.Wind;
                    return true;
                case ElementalType.Water:
                    lose = ElementalType.Fire;
                    return true;
                case ElementalType.Land:
                    lose = ElementalType.Water;
                    return true;
                case ElementalType.Wind:
                    lose = ElementalType.Land;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(win), win, null);
            }
        }

        public static bool TryGetLoseCase(this ElementalType lose, out ElementalType win)
        {
            switch (lose)
            {
                case ElementalType.Normal:
                    win = ElementalType.Normal;
                    return false;
                case ElementalType.Fire:
                    win = ElementalType.Water;
                    return true;
                case ElementalType.Water:
                    win = ElementalType.Land;
                    return true;
                case ElementalType.Land:
                    win = ElementalType.Wind;
                    return true;
                case ElementalType.Wind:
                    win = ElementalType.Fire;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lose), lose, null);
            }
        }

        public static ElementalResult GetBattleResult(this ElementalType from, ElementalType to)
        {
            if (from == ElementalType.Normal)
            {
                return ElementalResult.Draw;
            }

            if (from == to)
            {
                return ElementalResult.Draw;
            }

            if (from.TryGetWinCase(out var lose) && lose == to)
            {
                return ElementalResult.Win;
            }

            if (from.TryGetLoseCase(out var win) && win == to)
            {
                return ElementalResult.Lose;
            }

            return ElementalResult.Draw;
        }

        public static decimal GetMultiplier(this ElementalType from, ElementalType to)
        {
            var result = from.GetBattleResult(to);
            switch (result)
            {
                case ElementalResult.Win:
                    return WinMultiplier;
                case ElementalResult.Draw:
                case ElementalResult.Lose:
                    return 1m;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IReadOnlyList<ElementalType> GetAllTypes() =>
            _allTypes ??= new List<ElementalType>
            {
                ElementalType.Normal,
                ElementalType.Fire,
                ElementalType.Water,
                ElementalType.Land,
                ElementalType.Wind,
            };
    }
}
