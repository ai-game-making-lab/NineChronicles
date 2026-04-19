using System;
using System.Linq;
using Nekoyume.Blockchain;
using Nekoyume.Helper;
using Nekoyume.SingleClient;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient
{
    public class SingleClientModeTest
    {
        [Test]
        public void IsEnabledByCommandLineOption()
        {
            var options = new CommandLineOptions
            {
                SingleClient = true
            };

            Assert.IsTrue(SingleClientMode.IsEnabled(options));
        }

        [Test]
        public void IsEnabledByDefault()
        {
            Assert.IsTrue(SingleClientMode.IsEnabled(new CommandLineOptions()));
            Assert.IsTrue(SingleClientMode.IsEnabled(Array.Empty<string>(), null));
        }

        [Test]
        public void IsDisabledByExplicitRpcClient()
        {
            var options = new CommandLineOptions
            {
                RpcClient = true
            };

            Assert.IsFalse(SingleClientMode.IsEnabled(options));
            Assert.IsFalse(SingleClientMode.IsEnabled(
                new[] { "NineChronicles.exe", "--rpc-client" },
                null));
        }

        [TestCase("1")]
        [TestCase("true")]
        [TestCase("yes")]
        [TestCase("on")]
        public void IsEnabledByEnvironmentValue(string value)
        {
            Assert.IsTrue(SingleClientMode.IsEnabled(Array.Empty<string>(), value));
        }

        [Test]
        public void IsEnabledByCommandLineFlag()
        {
            Assert.IsTrue(SingleClientMode.IsEnabled(
                new[] { "NineChronicles.exe", "--single-client" },
                null));
        }

        [Test]
        public void ConfigureCommandLineOptionsDisablesRpc()
        {
            var options = new CommandLineOptions
            {
                SingleClient = true,
                RpcClient = true,
                RpcServerHost = "remote.example.com",
                RpcServerHosts = new[] { "remote-a.example.com", "remote-b.example.com" },
                RpcServerPort = 23061
            };

            SingleClientMode.ConfigureCommandLineOptions(
                options,
                "local-store",
                "local-private-key");

            Assert.IsFalse(options.RpcClient);
            Assert.IsNull(options.RpcServerHost);
            Assert.IsEmpty(options.RpcServerHosts.ToArray());
            Assert.AreEqual(0, options.RpcServerPort);
            Assert.AreEqual("local-store", options.StoragePath);
            Assert.AreEqual("local-private-key", options.PrivateKey);
        }

        [Test]
        public void ConfigureCommandLineOptionsPreservesExplicitLocalValues()
        {
            var options = new CommandLineOptions
            {
                SingleClient = true,
                StoragePath = "custom-store",
                PrivateKey = "custom-private-key"
            };

            SingleClientMode.ConfigureCommandLineOptions(
                options,
                "local-store",
                "local-private-key");

            Assert.AreEqual("custom-store", options.StoragePath);
            Assert.AreEqual("custom-private-key", options.PrivateKey);
        }

        [Test]
        public void RpcAgentSkipsGrpcChannelInitializationBySingleClientFlag()
        {
            Assert.IsFalse(RPCAgent.ShouldInitializeRpcTransport(
                new[] { "NineChronicles.exe", "--single-client" },
                null));
        }

        [Test]
        public void RpcAgentSkipsGrpcChannelInitializationBySingleClientEnvironment()
        {
            Assert.IsFalse(RPCAgent.ShouldInitializeRpcTransport(
                Array.Empty<string>(),
                "true"));
        }

        [Test]
        public void RpcAgentSkipsGrpcChannelInitializationByDefault()
        {
            Assert.IsFalse(RPCAgent.ShouldInitializeRpcTransport(
                Array.Empty<string>(),
                null));
        }

        [Test]
        public void RpcAgentInitializesGrpcChannelByExplicitRpcClientFlag()
        {
            Assert.IsTrue(RPCAgent.ShouldInitializeRpcTransport(
                new[] { "NineChronicles.exe", "--rpc-client" },
                null));
        }
    }
}
