using System;
using Nekoyume.SingleClient.Models.Market;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>ProductSellerMail</c> — market-system seller settlement.
    public sealed class ProductSellerMailSnapshot : MailSnapshot
    {
        public Guid ProductId { get; set; }
        public ProductSnapshot Product { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}
