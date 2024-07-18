using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UWP
using Steamworks;
#endif

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SteamManager : Instancable<SteamManager>
{
    public static uint appId { get; } = 480;
#if !UWP
    public void Awake()
    {
        base.Awake();
        if (SteamClient.RestartAppIfNecessary(appId))
        {
#if !DEBUG
            Application.Quit();
#endif
        }
        if(!SteamClient.IsValid)
            SteamClient.Init(appId);
    }

    private void Update()
    {
        if (SteamClient.IsValid)
            SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        if (SteamClient.IsValid)
            SteamClient.Shutdown();
    }
#endif
}