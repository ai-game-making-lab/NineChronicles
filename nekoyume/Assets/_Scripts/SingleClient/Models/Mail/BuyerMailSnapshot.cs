namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>BuyerMail</c> — legacy shop purchase confirmation.
    public sealed class BuyerMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.Auction;
    }
}
