namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>MonsterCollectionMail</c> — staking-round reward delivery.
    public sealed class MonsterCollectionMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.System;
    }
}
