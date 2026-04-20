#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.IO;
using UnityEngine;

namespace Nekoyume.SingleClient
{
    public static class SingleClientPaths
    {
        public const string DirectoryName = "SingleClient";
        public const string StateFileName = "local-state.json";
        public const string StoreDirectoryName = "store";

        public static string GetDefaultStatePath()
        {
            return Path.Combine(
                Application.persistentDataPath,
                DirectoryName,
                StateFileName);
        }

        public static string GetDefaultStorePath()
        {
            return Path.Combine(
                Application.persistentDataPath,
                DirectoryName,
                StoreDirectoryName);
        }
    }
}

#endif
