#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nekoyume.SingleClient.Editor
{
    /// <summary>
    /// Auto-imports TextMeshPro Essential Resources on Editor startup.
    /// Mirrors the manual menu "Window > Text Mesh Pro > Import TMP Essential
    /// Resources" so that a fresh clone or a Unity 6 version bump doesn't throw
    /// the noisy LogError <c>"TextMesh Pro Essential Resources are missing"</c>
    /// from <c>TMP_PackageResourceImporter.OnDestroy</c>.
    ///
    /// Synchronous filesystem scan (no async Client.List) so it completes even
    /// in <c>-batchmode -quit</c>. Candidate paths checked:
    ///   1. <c>Library/PackageCache/com.unity.ugui*/Package Resources/TMP Essential Resources.unitypackage</c>
    ///   2. <c>Packages/com.unity.ugui*/Package Resources/TMP Essential Resources.unitypackage</c>
    ///
    /// Sentinel: <c>Library/single-client-tmp-imported</c>. Menu
    /// <c>SingleClient/Re-import TMP Essential Resources</c> forces re-import.
    /// </summary>
    [InitializeOnLoad]
    public static class SingleClientTmpEssentialsImporter
    {
        private const string Sentinel = "Library/single-client-tmp-imported";
        private const string EssentialsRelativePath = "Package Resources/TMP Essential Resources.unitypackage";
        private const string MenuItem = "SingleClient/Re-import TMP Essential Resources";

        static SingleClientTmpEssentialsImporter()
        {
            if (File.Exists(Sentinel))
            {
                return;
            }

            TryImport(force: false);
        }

        [MenuItem(MenuItem)]
        public static void ForceReimport()
        {
            TryImport(force: true);
        }

        private static void TryImport(bool force)
        {
            var essentials = FindEssentialsPackage();
            if (string.IsNullOrEmpty(essentials))
            {
                Debug.LogWarning("[TMP] Essentials package not located in Library/PackageCache or Packages.");
                return;
            }

            Debug.Log($"[TMP] Auto-importing Essentials from {essentials}  (force={force})");
            try
            {
                AssetDatabase.ImportPackage(essentials, interactive: false);
                File.WriteAllText(Sentinel, DateTime.UtcNow.ToString("O"));
            }
            catch (Exception e)
            {
                Debug.LogError($"[TMP] Import failed: {e.Message}");
            }
        }

        private static string FindEssentialsPackage()
        {
            foreach (var root in new[] { "Library/PackageCache", "Packages" })
            {
                if (!Directory.Exists(root))
                {
                    continue;
                }

                foreach (var dir in Directory.GetDirectories(root))
                {
                    var name = Path.GetFileName(dir);
                    if (name == null || !name.StartsWith("com.unity.ugui", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var candidate = Path.Combine(dir, EssentialsRelativePath);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }
    }
}
#endif
