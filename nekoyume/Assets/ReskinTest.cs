using UnityEngine;
using UnityEditor;

public class ReskinTest
{
    public static void RunTest()
    {
        Debug.Log("=== Reskin Test Started ===");

        // Check reskinned assets
        string[] reskinnedAssets = new string[]
        {
            "Assets/AddressableAssets/Character/FullCostume/40100000/40100000_reskin.png",
            "Assets/AddressableAssets/Character/FullCostume/40100000/40100000.png"
        };

        foreach (string assetPath in reskinnedAssets)
        {
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath) != null)
            {
                Debug.Log($"[PASS] Asset found: {assetPath}");

                // Get texture info
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                Debug.Log($"  Size: {tex.width}x{tex.height}");
                Debug.Log($"  Format: {tex.format}");
            }
            else
            {
                Debug.LogError($"[FAIL] Asset not found: {assetPath}");
            }
        }

        Debug.Log("=== Reskin Test Complete ===");

        // Exit Unity
        EditorApplication.Exit(0);
    }
}
