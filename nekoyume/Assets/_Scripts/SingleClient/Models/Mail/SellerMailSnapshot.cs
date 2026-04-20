namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>SellerMail</c> — legacy shop sale settlement.
    public sealed class SellerMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.Auction;
    }
}
