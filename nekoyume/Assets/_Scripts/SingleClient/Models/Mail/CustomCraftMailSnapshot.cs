#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>CustomCraftMail</c> — custom-craft workshop equipment delivery. Reuses
    /// the <see cref="AttachmentMailSnapshot.Attachment"/> <see cref="Items.IItemSnapshot"/>
    /// slot so UI can render the equipment payload identically to other attachment mails.
    public sealed class CustomCraftMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.CustomCraft;
    }
}

#endif
