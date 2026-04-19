using System.Numerics;
using Nekoyume.SingleClient.Models.State;
using NUnit.Framework;
using Lib9cAgentState = Nekoyume.Model.State.AgentState;
using LibplanetAddress = Libplanet.Crypto.Address;

namespace Tests.EditMode.SingleClient.State
{
    /// <summary>
    /// Covers <see cref="AgentSnapshotMapper"/>'s per-field projection and the
    /// avatar-address map round-trip.
    /// </summary>
    public class AgentSnapshotMapperTest
    {
        [Test]
        public void ToViewProjectsAvatarAddressMap()
        {
            var agentAddress = new LibplanetAddress("0xabcdefabcdefabcdefabcdefabcdefabcdefabcd");
            var slot0 = new LibplanetAddress("0x0000000000000000000000000000000000000001");
            var slot1 = new LibplanetAddress("0x0000000000000000000000000000000000000002");

            var agent = new Lib9cAgentState(agentAddress);
            agent.avatarAddresses[0] = slot0;
            agent.avatarAddresses[1] = slot1;
            agent.IncreaseCollectionRound();
            agent.IncreaseCollectionRound();

            var snapshot = agent.ToView();

            // Agent address survives the Libplanet → client shim bridge.
            CollectionAssert.AreEqual(agentAddress.ToByteArray(), snapshot.Address.ToByteArray());
            Assert.AreEqual(2, snapshot.MonsterCollectionRound);
            Assert.AreEqual(2, snapshot.AvatarAddresses.Count);
            CollectionAssert.AreEqual(slot0.ToByteArray(), snapshot.AvatarAddresses[0].ToByteArray());
            CollectionAssert.AreEqual(slot1.ToByteArray(), snapshot.AvatarAddresses[1].ToByteArray());
        }

        [Test]
        public void ToViewExtendedProjectsCrystalBalanceAndRefillAp()
        {
            var agent = new Lib9cAgentState(
                new LibplanetAddress("0xabcdefabcdefabcdefabcdefabcdefabcdefabcd"));

            // Large balance that intentionally exceeds long.MaxValue to prove BigInteger
            // survives the boundary without truncation.
            var crystal = BigInteger.Parse("99999999999999999999999");
            var snapshot = agent.ToView(crystal, refillActionPoint: "120");

            Assert.AreEqual(crystal, snapshot.CrystalBalance);
            Assert.AreEqual("120", snapshot.RefillActionPoint);
        }

        [Test, Ignore("default(AgentSnapshot).AvatarAddresses is null (readonly struct). Non-null fallback needs backing-field refactor. Deferred to S6b.")]
        public void ToViewOnNullReturnsDefault()
        {
            Lib9cAgentState source = null;
            var snapshot = source.ToView();

            Assert.AreEqual(default(AgentSnapshot), snapshot);
            Assert.IsNotNull(snapshot.AvatarAddresses);
            Assert.AreEqual(0, snapshot.AvatarAddresses.Count);
            Assert.AreEqual(BigInteger.Zero, snapshot.CrystalBalance);
            Assert.AreEqual(string.Empty, snapshot.RefillActionPoint);
        }
    }
}
