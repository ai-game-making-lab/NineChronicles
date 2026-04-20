#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Nekoyume.SingleClient.Models.Market;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>ProductCancelMail</c> — market listing cancellation notice.
    public sealed class ProductCancelMailSnapshot : MailSnapshot
    {
        public Guid ProductId { get; set; }
        public ProductSnapshot Product { get; set; }

        public override MailType MailType => MailType.Auction;
    }
}

#endif
