#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Libplanet.Common;
using Libplanet.Crypto;

namespace Nekoyume.SingleClient
{
    public static class SingleClientPrivateKeyFactory
    {
        public static string CreatePrivateKeyHex()
        {
            var privateKey = new PrivateKey();
            return ByteUtil.Hex(privateKey.ByteArray).PadLeft(64, '0');
        }
    }
}

#endif
