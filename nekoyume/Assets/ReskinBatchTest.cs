using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ReskinBatchTest
{
    public static void RunTest()
    {
        Debug.Log("=== Reskin Batch Test Started ===");

        // Find all reskinned assets
        string[] guids = AssetDatabase.FindAssets("reskin", new[] { "Assets" });
        Debug.Log($"Found {guids.Length} reskinned assets");

        int loadedCount = 0;
        int failedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (tex != null)
            {
                loadedCount++;
                if (loadedCount <= 10) // Show first 10
                {
                    Debug.Log($"[PASS] {path} ({tex.width}x{tex.height}, {tex.format})");
                }
            }
            else
            {
                failedCount++;
                Debug.LogError($"[FAIL] {path}");
            }
        }

        Debug.Log($"=== Results ===");
        Debug.Log($"Total Reskinned: {guids.Length}");
        Debug.Log($"Loaded Successfully: {loadedCount}");
        Debug.Log($"Failed: {failedCount}");

        // Test specific assets
        Debug.Log($"=== Specific Asset Tests ===");
        TestAsset("Assets/AddressableAssets/Character/FullCostume/40100000/40100000_reskin.png");
        TestAsset("Assets/AddressableAssets/Character/FullCostume/40100001/40100001_reskin.png");

        Debug.Log("=== Reskin Batch Test Complete ===");
        EditorApplication.Exit(0);
    }

    private static void TestAsset(string path)
    {
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex != null)
        {
            Debug.Log($"[SPECIFIC PASS] {Path.GetFileName(path)} - {tex.width}x{tex.height}, {tex.format}");
        }
        else
        {
            Debug.LogError($"[SPECIFIC FAIL] {path}");
        }
    }
}
