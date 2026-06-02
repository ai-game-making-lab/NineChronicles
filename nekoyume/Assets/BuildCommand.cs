using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;

public class BuildCommand
{
    public static void BuildWindows()
    {
        Debug.Log("=== Starting NineChronicles Windows Build ===");

        string buildPath = "Builds/Windows";
        string locationPath = Path.Combine(buildPath, "NineChronicles.exe");

        // Create build directory
        Directory.CreateDirectory(buildPath);

        // Rebuild Addressables
        Debug.Log("Rebuilding Addressables...");
        AddressableAssetSettings.BuildPlayerContent();
        Debug.Log("Addressables build completed");

        // Build settings
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = locationPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        // Execute build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // Check result
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[SUCCESS] Build completed: {report.summary.totalSize} bytes");
            Debug.Log($"[SUCCESS] Build time: {report.summary.totalTime.TotalMinutes} minutes");
            Debug.Log($"[SUCCESS] Output: {locationPath}");
        }
        else
        {
            Debug.LogError($"[FAIL] Build failed: {report.summary.result}");
        }

        // Exit Unity
        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
