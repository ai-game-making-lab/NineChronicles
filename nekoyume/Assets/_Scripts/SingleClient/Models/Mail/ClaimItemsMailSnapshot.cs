using System.Collections.Generic;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>ClaimItemsMail</c> — batch claim delivering FAV and/or fungible items.
    public sealed class ClaimItemsMailSnapshot : MailSnapshot
    {
        public IReadOnlyList<FungibleAssetValue> FungibleAssetValues { get; set; }

        /// Pairs of (item id, count). Mirrors lib9c's <c>List&lt;(int, int)&gt;</c> shape.
        public IReadOnlyList<(int Id, int Count)> Items { get; set; }

        public string Memo { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}
