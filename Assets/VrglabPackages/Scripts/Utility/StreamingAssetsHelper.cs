using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class StreamingAssetsHelper
{
    /// <summary>
    /// Returns the immediate children in Assets/StreamingAssets/subFolder      (no recursion)
    /// Works on Android device or editor.
    /// </summary>
    public static string[] ListFiles(string subFolder)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 1. Get the Java AssetManager
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity    = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var assetMgr    = activity.Call<AndroidJavaObject>("getAssets");

        // 2. Call AssetManager.list(String path) → String[]
        string[] names  = assetMgr.Call<string[]>("list", subFolder);

        // 3. Prepend the real StreamingAssets path so you can open them later with UnityWebRequest
        for (int i = 0; i < names.Length; ++i)
        {
            names[i] = System.IO.Path.Combine(Application.streamingAssetsPath, subFolder, names[i]);
        }
        return names;
#else
        // Editor / standalone: simple
        string folder = System.IO.Path.Combine(Application.streamingAssetsPath, subFolder);
        return System.IO.Directory.GetFiles(folder);
#endif
    }


    /// <summary>
    /// Reads a whole text file and returns its contents.
    /// * Editor / Stand‑alone: identical to File.ReadAllText.
    /// * Android + StreamingAssets: transparently uses UnityWebRequest.
    /// 
    /// Blocks the caller in all cases; if you prefer async/await,
    /// use ReadAllTextAsync instead.
    /// </summary>
    public static string ReadAllText(string path) =>
#if UNITY_ANDROID && !UNITY_EDITOR
        ReadAllTextAsync(path).GetAwaiter().GetResult();
#else
        File.ReadAllText(path);
#endif

    /// <summary>
    /// Async/await version (doesn’t block the main thread).
    /// </summary>
    public static async Task<string> ReadAllTextAsync(string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Is it inside StreamingAssets?  (Add whatever test you like.)
        bool isInStreamingAssets = path.StartsWith(Application.streamingAssetsPath, StringComparison.Ordinal);

        if (isInStreamingAssets)
        {
            using (var req = UnityWebRequest.Get(path))
            {
#if UNITY_2023_2_OR_NEWER
                await req.SendWebRequest();
#else
                var op = req.SendWebRequest();
                while (!op.isDone) await Task.Yield();
#endif
                if (req.result != UnityWebRequest.Result.Success)
                    throw new IOException(req.error);

                return req.downloadHandler.text;
            }
        }
#endif
        // Fallback – works in Editor, Windows, macOS, Linux, iOS, and also on Android
        // *outside* the StreamingAssets jar.
        return await Task.Run(() => File.ReadAllText(path));
    }
}
