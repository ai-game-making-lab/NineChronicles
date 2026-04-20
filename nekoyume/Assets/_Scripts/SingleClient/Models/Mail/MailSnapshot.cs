using System;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Abstract projection of lib9c <c>Nekoyume.Model.Mail.Mail</c>. Only carries the four base
    /// fields every UI consumer reads; subtype-specific payloads live on the derived snapshots.
    public abstract class MailSnapshot
    {
        public Guid Id { get; set; }
        public long BlockIndex { get; set; }
        public long RequiredBlockIndex { get; set; }
        public bool New { get; set; }

        /// Overridden per subtype so UI category filters can inspect the originating system
        /// without a type-switch. Matches <c>Mail.MailType</c> on the lib9c side.
        public virtual MailType MailType => MailType.System;
    }
}
