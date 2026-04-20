namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of <c>DailyRewardMail</c> — daily AP refresh result.
    public sealed class DailyRewardMailSnapshot : AttachmentMailSnapshot
    {
        public override MailType MailType => MailType.System;
    }
}
