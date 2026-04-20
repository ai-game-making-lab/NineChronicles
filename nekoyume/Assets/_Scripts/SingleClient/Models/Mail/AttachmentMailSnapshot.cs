using Nekoyume.SingleClient.Models.Items;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Common base for lib9c's <c>AttachmentMail</c> family (mail carrying a single crafted /
    /// enhanced item). The concrete lib9c <c>AttachmentActionResult</c> graph is collapsed to the
    /// <see cref="Attachment"/> <see cref="IItemSnapshot"/> since UI only reads the resulting item.
    public abstract class AttachmentMailSnapshot : MailSnapshot
    {
        public IItemSnapshot Attachment { get; set; }
    }
}
