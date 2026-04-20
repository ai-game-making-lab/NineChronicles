#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nekoyume.SingleClient.Models.Mail
{
    /// Projection of lib9c <c>MailBox</c> — the avatar-scoped mailbox. Enumerates in
    /// descending <see cref="MailSnapshot.BlockIndex"/> order to match lib9c's iteration
    /// contract (newest first). Backing store is a <see cref="List{T}"/>; the mapper fills it
    /// once per snapshot and downstream UI treats the container as read-only.
    public sealed class MailBoxSnapshot : IEnumerable<MailSnapshot>
    {
        private readonly List<MailSnapshot> _mails;

        public MailBoxSnapshot()
        {
            _mails = new List<MailSnapshot>();
        }

        public MailBoxSnapshot(IEnumerable<MailSnapshot> mails)
        {
            _mails = mails?.ToList() ?? new List<MailSnapshot>();
        }

        public int Count => _mails.Count;

        public MailSnapshot this[int index] => _mails[index];

        public IEnumerator<MailSnapshot> GetEnumerator() =>
            _mails.OrderByDescending(m => m.BlockIndex).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

#endif
