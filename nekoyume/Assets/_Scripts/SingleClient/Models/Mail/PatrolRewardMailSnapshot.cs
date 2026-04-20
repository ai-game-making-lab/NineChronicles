#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>PatrolRewardMail</c> — patrol-system periodic reward bundle. Mirrors
    /// both the FAV list and item (id, count) list; either may be empty in a given payload.
    /// Shape parallels <see cref="ClaimItemsMailSnapshot"/> since both collapse the same lib9c
    /// multi-reward encoding.
    public sealed class PatrolRewardMailSnapshot : MailSnapshot
    {
        public IReadOnlyList<FungibleAssetValue> FungibleAssetValues { get; set; }

        /// Pairs of (item id, count). Mirrors lib9c's <c>List&lt;(int, int)&gt;</c> shape.
        public IReadOnlyList<(int Id, int Count)> Items { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}

#endif
