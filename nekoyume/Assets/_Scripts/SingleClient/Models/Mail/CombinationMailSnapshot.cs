namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>CombinationMail</c> — a workshop-crafted item delivered via mail.
    public sealed class CombinationMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.Workshop;
    }
}
