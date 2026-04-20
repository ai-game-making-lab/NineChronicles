#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>WorldBossRewardMail</c> — worldboss season-clear reward bundle. Shape
    /// parallels <see cref="PatrolRewardMailSnapshot"/>; the <see cref="MailType"/> is
    /// <see cref="MailType.System"/> to match lib9c's worldboss routing.
    public sealed class WorldBossRewardMailSnapshot : MailSnapshot
    {
        public IReadOnlyList<FungibleAssetValue> FungibleAssetValues { get; set; }

        /// Pairs of (item id, count). Mirrors lib9c's <c>List&lt;(int, int)&gt;</c> shape.
        public IReadOnlyList<(int Id, int Count)> Items { get; set; }

        public override MailType MailType => MailType.System;
    }
}

#endif
