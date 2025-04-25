using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UWP && !UNITY_ANDROID && !UNITY_IPHONE
using Steamworks;
#endif

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SteamManager : Instancable<SteamManager>
{
    public static uint appId { get; } = 3376920;



#if !UWP && !UNITY_ANDROID && !UNITY_IPHONE
    public void Awake()
    {
        base.Awake();
        try
        {
            SteamClient.Init(appId);
        }
        catch (Exception e)
        {
#if !DEBUG
            Application.Quit();
#else
            Debug.LogException(e);
#endif
        }


        if (!SteamApps.IsAppInstalled(appId) || 
            SteamApps.IsVACBanned || 
            SteamParental.BIsAppInBlockList(appId) || 
            SteamParental.IsAppBlocked(appId) ||
            SteamClient.RestartAppIfNecessary(appId))
        {
#if !DEBUG
            Application.Quit();
#else
            Debug.LogError("If this was production app would exit");
#endif
        }
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