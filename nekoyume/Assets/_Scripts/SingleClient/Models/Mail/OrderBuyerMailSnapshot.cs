using System;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>OrderBuyerMail</c> — order-system buyer settlement.
    public sealed class OrderBuyerMailSnapshot : MailSnapshot
    {
        public Guid OrderId { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}
