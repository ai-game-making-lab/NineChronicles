#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using Nekoyume.Helper;

namespace Nekoyume.SingleClient
{
    public static class SingleClientMode
    {
        public const string CommandLineFlag = "--single-client";
        public const string EnvironmentVariable = "NC_SINGLE_CLIENT";
        private const string RpcClientCommandLineFlag = "--rpc-client";

        public static bool IsEnabled(CommandLineOptions options)
        {
            if (options?.SingleClient == true)
            {
                return true;
            }

            if (options?.RpcClient == true)
            {
                return false;
            }

            return options is null ||
                options.Empty ||
                IsEnabled(
                    Environment.GetCommandLineArgs(),
                    Environment.GetEnvironmentVariable(EnvironmentVariable));
        }

        public static bool IsEnabled(
            IEnumerable<string> commandLineArgs,
            string environmentValue)
        {
            if (IsTruthy(environmentValue))
            {
                return true;
            }

            if (commandLineArgs is null)
            {
                return true;
            }

            foreach (var arg in commandLineArgs)
            {
                if (string.Equals(arg, CommandLineFlag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            foreach (var arg in commandLineArgs)
            {
                if (string.Equals(arg, RpcClientCommandLineFlag, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public static void ConfigureCommandLineOptions(CommandLineOptions options)
        {
            ConfigureCommandLineOptions(
                options,
                SingleClientPaths.GetDefaultStorePath(),
                null);
        }

        public static void ConfigureCommandLineOptions(
            CommandLineOptions options,
            string storagePath,
            string privateKeyHex)
        {
            if (!IsEnabled(options))
            {
                return;
            }

            options.RpcClient = false;
            options.RpcServerHost = null;
            options.RpcServerHosts = Array.Empty<string>();
            options.RpcServerPort = 0;

            if (string.IsNullOrWhiteSpace(options.StoragePath) &&
                !string.IsNullOrWhiteSpace(storagePath))
            {
                options.StoragePath = storagePath;
            }

            if (string.IsNullOrWhiteSpace(options.PrivateKey) &&
                !string.IsNullOrWhiteSpace(privateKeyHex))
            {
                options.PrivateKey = privateKeyHex;
            }
        }

        private static bool IsTruthy(string value)
        {
            return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "on", StringComparison.OrdinalIgnoreCase);
        }
    }
}

#endif
