namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>ItemEnhanceMail</c> — an upgraded equipment delivered via mail.
    public sealed class ItemEnhanceMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.Workshop;
    }
}
