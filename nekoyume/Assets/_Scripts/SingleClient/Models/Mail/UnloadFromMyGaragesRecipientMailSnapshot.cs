using System.Collections.Generic;
using System.Security.Cryptography;
using Nekoyume.SingleClient.Blockchain;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>UnloadFromMyGaragesRecipientMail</c> — cross-avatar garage unload
    /// notification. Pairs are flattened to simple tuples; UI just iterates and renders them.
    public sealed class UnloadFromMyGaragesRecipientMailSnapshot : MailSnapshot
    {
        /// Per-balance address FAV payouts. <see langword="null"/> when the mail carries no FAV.
        public IReadOnlyList<(Address BalanceAddr, FungibleAssetValue Value)> FungibleAssetValues { get; set; }

        /// Per-fungible-id stack counts. <see langword="null"/> when the mail carries no items.
        public IReadOnlyList<(HashDigest<SHA256Algorithm> FungibleId, int Count)> FungibleIdAndCounts { get; set; }

        public string Memo { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}
