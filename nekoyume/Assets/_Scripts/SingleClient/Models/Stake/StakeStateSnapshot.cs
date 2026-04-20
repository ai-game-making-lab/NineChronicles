using System.Collections.Generic;

namespace Nekoyume.SingleClient.Models.Stake
{
    /// Projection of lib9c <c>StakeState</c> V2/V3. Only the three scalar fields that
    /// <c>States.StakeStateV2</c> consumers read are carried — the derived
    /// <c>ClaimableBlockIndex</c> / <c>CancellableBlockIndex</c> helpers rely on lib9c's
    /// <c>Contract</c> so they stay on the legacy reference until the stake-contract mirror lands.
    /// <see cref="Achievements"/> is included as a scaffold (lib9c V2 dropped it; the slot is
    /// here so legacy-state reads can surface the migrated value without a schema change).
    public sealed class StakeStateSnapshot
    {
        public long StartedBlockIndex { get; set; }
        public long ReceivedBlockIndex { get; set; }

        /// Per-milestone reward-claim flags from the legacy v1 StakeState; empty on v2+.
        public IReadOnlyList<int> Achievements { get; set; }
    }
}
