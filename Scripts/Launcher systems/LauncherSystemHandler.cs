using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UWP || MICROSOFT_GAME_CORE
using Microsoft.Xbox;
using XGamingRuntime;
#endif

#if PC && !MICROSOFT_GAME_CORE
using Steamworks;
#else
using Steamworks;
#endif


public struct Achievemnt
{
    public int AchievemntID;
}


public class LauncherSystemHandler : Instancable<LauncherSystemHandler>
{
    public GameObject SteamManager, XboxManager;

    private void OnEnable()
    {
#if UWP || MICROSOFT_GAME_CORE
        SteamManager.SetActive(false);
        XboxManager.SetActive(true);
#endif
#if PC && !MICROSOFT_GAME_CORE
        SteamManager.SetActive(true);
        XboxManager.SetActive(false);
#else
        SteamManager.SetActive(true);
        XboxManager.SetActive(false);
#endif
    }

    public void UnlockAchivement(string Arg)
    {
#if UWP || MICROSOFT_GAME_CORE
        Gdk.Helpers.UnlockAchievement(Arg);
#endif
#if PC && !MICROSOFT_GAME_CORE
        SteamAchievements.Instance.Achieve(Arg);
#else
        SteamAchievements.Instance.Achieve(Arg);
#endif
    }

    public object GetLanguage()
    {
#if UWP || MICROSOFT_GAME_CORE
        SDK.XPackageGetUserLocale(out string locale);
        Debug.Log(locale);
        return locale;
#endif
#if PC && !MICROSOFT_GAME_CORE
        return SteamApps.GameLanguage;
#else
        return SteamApps.GameLanguage;
#endif
    }
}
