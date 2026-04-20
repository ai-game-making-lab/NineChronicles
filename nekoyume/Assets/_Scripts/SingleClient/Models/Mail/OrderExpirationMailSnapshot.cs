#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>OrderExpirationMail</c> — order-system listing-expired notice. Mirrors
    /// the same minimal surface as <see cref="CancelOrderMailSnapshot"/>; UI branches on the
    /// concrete type rather than inspecting distinct fields.
    public sealed class OrderExpirationMailSnapshot : MailSnapshot
    {
        public Guid OrderId { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}

#endif
