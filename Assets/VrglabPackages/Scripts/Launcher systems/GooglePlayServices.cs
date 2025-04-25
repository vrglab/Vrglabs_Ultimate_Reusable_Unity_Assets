using FMODUnity;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
using UnityEngine;
using UnityEngine.Events;

public class GooglePlayServices : Instancable<GooglePlayServices>
{
    public UnityEvent SuccessFullAuth;
    public UnityEvent FailedAuth;
#if UNITY_ANDROID
    private PlayGamesPlatform platform;
#endif
    private void OnEnable()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.InitializeNearby((connection) =>
        {
        });
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        platform = PlayGamesPlatform.Instance;
        Authenticate();
#endif
    }


    public void Authenticate()
    {
#if UNITY_ANDROID
        platform.Authenticate((status) =>
        {
            switch (status)
            {
                case GooglePlayGames.BasicApi.SignInStatus.Success:
                    SuccessFullAuth.Invoke();
                    break;
                default:
                    FailedAuth.Invoke();
                    break;
            }
        });
#endif
    }

    public void UnlockAchievment(Achievment ach)
    {
#if UNITY_ANDROID
        if(platform.IsAuthenticated())
        {
            platform.UnlockAchievement(ach.googleplay_id);
        }
#endif
    }

    public bool CheckIsAuthenticated()
    {
#if UNITY_ANDROID
        return platform.IsAuthenticated();
#else 
        return false;
#endif
    }

    public string GetActiveUsername()
    {
#if UNITY_ANDROID
       return platform.GetUserDisplayName();
#else
        return "";
#endif
    }
}
