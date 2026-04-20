#if LIB9C_RESTORED // stubbed out after lib9c deletion
using LibplanetAddress = Libplanet.Crypto.Address;

namespace Nekoyume.SingleClient.Blockchain
{
    /// <summary>
    /// Extension bridge between the SingleClient 20-byte <see cref="Address"/> struct and
    /// <see cref="LibplanetAddress"/>. Lets 146 <c>States.Instance</c> consumers adopt the
    /// <c>ClientStateViewProvider</c> facade without waiting on every downstream API
    /// (WorldBoss, Staking, Agent, Market) to grow a client-Address overload — migrating call
    /// sites pipe <c>snapshot.Address.ToLibplanet()</c> at the boundary, keeping the facade's
    /// DTO surface lib9c-free while bridging at the call site.
    /// </summary>
    public static class AddressBridgeExtensions
    {
        public static LibplanetAddress ToLibplanet(this Address client)
        {
            return new LibplanetAddress(client.ToByteArray());
        }

        public static Address ToClient(this LibplanetAddress libplanet)
        {
            return new Address(libplanet.ToByteArray());
        }
    }
}

#endif
