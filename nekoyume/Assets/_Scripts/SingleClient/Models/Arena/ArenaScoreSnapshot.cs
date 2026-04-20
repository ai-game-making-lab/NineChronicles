#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Arena
{
    /// Projection of <c>ArenaScore</c>. Only the identity + score are carried — the mutating
    /// <c>AddScore</c> helper on the lib9c type is intentionally omitted because UI just reads
    /// the score.
    public sealed class ArenaScoreSnapshot
    {
        public const int ArenaScoreDefault = 1000;

        public Address Address { get; set; }
        public int Score { get; set; } = ArenaScoreDefault;
    }
}

#endif
