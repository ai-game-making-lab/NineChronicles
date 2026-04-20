#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>CancelOrderMail</c> — order-system cancellation notice. Only the order
    /// id is needed by UI (the item payload lives on the related <c>SellCancelMail</c> which is
    /// mirrored separately).
    public sealed class CancelOrderMailSnapshot : MailSnapshot
    {
        public Guid OrderId { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}

#endif
