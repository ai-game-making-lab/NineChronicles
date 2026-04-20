#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Market;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>ProductBuyerMail</c> — market-system buyer settlement. Carries the full
    /// <see cref="ProductSnapshot"/> so UI can <c>is ItemProductSnapshot</c> / <c>is FavProductSnapshot</c>
    /// discriminate as it does today.
    public sealed class ProductBuyerMailSnapshot : MailSnapshot
    {
        public Guid ProductId { get; set; }
        public ProductSnapshot Product { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}

#endif
