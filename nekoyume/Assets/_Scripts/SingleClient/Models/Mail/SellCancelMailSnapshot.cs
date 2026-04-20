namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>SellCancelMail</c> — legacy shop sell-cancellation delivery. The
    /// refunded equipment/consumable is surfaced through the base
    /// <see cref="AttachmentMailSnapshot.Attachment"/> slot.
    public sealed class SellCancelMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.Auction;
    }
}
