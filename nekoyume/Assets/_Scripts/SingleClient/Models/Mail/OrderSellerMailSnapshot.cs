using System;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>OrderSellerMail</c> — order-system seller settlement.
    public sealed class OrderSellerMailSnapshot : MailSnapshot
    {
        public Guid OrderId { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}
