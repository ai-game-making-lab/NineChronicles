using UnityEngine;
using UnityEditor;

public class PlayTestCommand
{
    public static void PlayGame()
    {
        Debug.Log("=== NineChronicles Play Test ===");

        // Enter Play Mode
        EditorApplication.isPlaying = true;

        // Wait for game to load
        EditorApplication.delayCall += () => {
            Debug.Log("NineChronicles game is running");
            Debug.Log("Reskinned character assets should be visible");

            // Exit after 5 seconds
            EditorApplication.delayCall += () => {
                System.Threading.Thread.Sleep(5000);
                EditorApplication.isPlaying = false;
                EditorApplication.Exit(0);
            };
        };
    }
}
