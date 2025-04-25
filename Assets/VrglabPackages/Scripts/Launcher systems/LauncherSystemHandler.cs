using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UWP || MICROSOFT_GAME_CORE
using Unity.XGamingRuntime;
#endif

#if PC && !MICROSOFT_GAME_CORE
using Steamworks;
#endif

#if GAMEJOLT
using GameJolt;
#endif


public struct Achievemnt
{
    public int AchievemntID;
}


public class LauncherSystemHandler : Instancable<LauncherSystemHandler>
{
    public GameObject SteamManager, XboxManager, GameJoltManager, GooglePlayManager;

    private void OnEnable()
    {
#if UWP || MICROSOFT_GAME_CORE
        SteamManager.SetActive(false);
        GameJoltManager.SetActive(false);
        XboxManager.SetActive(true);
#endif
#if PC && !MICROSOFT_GAME_CORE
        SteamManager.SetActive(true);
        GameJoltManager.SetActive(false);
        XboxManager.SetActive(false);
#endif
#if GAMEJOLT
        SteamManager.SetActive(false);
        GameJoltManager.SetActive(true);
        XboxManager.SetActive(false);
#endif
#if UNITY_ANDROID
        SteamManager.SetActive(false);
        GameJoltManager.SetActive(false);
        XboxManager.SetActive(false);
        GooglePlayManager.SetActive(true);
#endif
    }

    public void UnlockAchivement(Achievment Arg)
    {
#if UWP || MICROSOFT_GAME_CORE
        
#endif
#if PC && !MICROSOFT_GAME_CORE && !GAMEJOLT
        SteamAchievements.Instance.Achieve(Arg.steam_id);
#endif
#if GAMEJOLT
        TrophyManager.Instance.Unlock(Arg);
#endif

#if UNITY_ANDROID
        GooglePlayServices.Instance.UnlockAchievment(Arg);
#endif
        Arg.unlocked = true;
    }

    public object GetLanguage()
    {
#if UWP || MICROSOFT_GAME_CORE
        string locale = GDK.Instance.XGetLocale();
        Debug.Log(locale);
        return locale;
#elif PC && !MICROSOFT_GAME_CORE
        return SteamApps.GameLanguage;
#else
        return Application.systemLanguage.ToIsoCode();
#endif
    }
}
