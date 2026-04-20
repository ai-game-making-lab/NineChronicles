#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.L10n
{
    public class L10nNotInitializedException : Exception
    {
    }

    public class L10nAlreadyContainsKeyException : Exception
    {
        public L10nAlreadyContainsKeyException(string message) : base(message)
        {
        }
    }
}

#endif
