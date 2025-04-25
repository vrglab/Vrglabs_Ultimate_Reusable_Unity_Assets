using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;


#if UWP || MICROSOFT_GAME_CORE
using Unity.XGamingRuntime;
#endif

#if PC && !MICROSOFT_GAME_CORE
using Steamworks;
#endif

namespace conditions.json
{
    public class IsInputDevice
    {
        public static bool IsPc()
        {
#if UWP || MICROSOFT_GAME_CORE
            return Gamepad.current == null;
#elif PC && !MICROSOFT_GAME_CORE || GAMEJOLT
            return Keyboard.current != null;
#elif UNITY_ANDROID || UNITY_IPHONE
            return false;
#else
            return false;
#endif
        }

        public static bool IsGamepad()
        {
            return Gamepad.current != null;
        }

        public static bool IsTouch()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            return true;
#else
            return false;
#endif
        }
    }

    public class UserData
    {
        public static bool IsUserLoggedIn()
        {
#if PC && !MICROSOFT_GAME_CORE
            return SteamClient.IsLoggedOn;
#elif UWP || MICROSOFT_GAME_CORE
            return GDK.Instance.IsLoggedIn;
#elif GAMEJOLT
            return  (GameJolt.API.GameJoltAPI.Instance.CurrentUser != null && GameJolt.API.GameJoltAPI.Instance.CurrentUser.IsAuthenticated);
#elif UNITY_ANDROID
            return GooglePlayServices.Instance.CheckIsAuthenticated();
#else
            return true;
#endif
        }
    }
}
