namespace Nekoyume.SingleClient.Combat
{
    /// <summary>
    /// Level-derived combat stats for a single-client avatar. Kept as a pure
    /// function of level so <see cref="SingleClientAvatarState"/> does not need
    /// to persist HP/ATK/DEF columns — the table below is canonical.
    /// </summary>
    public static class SingleClientAvatarStats
    {
        public const long BaseHp = 100L;
        public const long HpPerLevel = 20L;

        public const long BaseAtk = 10L;
        public const long AtkPerLevel = 3L;

        public const long BaseDef = 5L;
        public const long DefPerLevel = 2L;

        public static long HpAtLevel(int level)
        {
            var l = level < 1 ? 1 : level;
            return BaseHp + HpPerLevel * (l - 1);
        }

        public static long AtkAtLevel(int level)
        {
            var l = level < 1 ? 1 : level;
            return BaseAtk + AtkPerLevel * (l - 1);
        }

        public static long DefAtLevel(int level)
        {
            var l = level < 1 ? 1 : level;
            return BaseDef + DefPerLevel * (l - 1);
        }

        /// <summary>Exp required to advance from <c>level</c> → <c>level+1</c>.</summary>
        public static long ExpToNextLevel(int level)
        {
            var l = level < 1 ? 1 : level;
            return 100L * l;
        }

        /// <summary>
        /// Applies <paramref name="expGained"/> to the avatar, leveling up as
        /// many times as thresholds are crossed. Returns the number of levels
        /// gained. Remainder exp carries over.
        /// </summary>
        public static int GrantExp(SingleClientAvatarState avatar, long expGained)
        {
            if (avatar is null || expGained <= 0)
            {
                return 0;
            }

            avatar.exp += expGained;

            var levelsGained = 0;
            while (true)
            {
                var threshold = ExpToNextLevel(avatar.level);
                if (avatar.exp < threshold)
                {
                    break;
                }

                avatar.exp -= threshold;
                avatar.level++;
                levelsGained++;
            }

            return levelsGained;
        }
    }
}
