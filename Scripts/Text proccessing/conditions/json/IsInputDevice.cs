using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;


#if UWP || MICROSOFT_GAME_CORE
using Microsoft.Xbox;
using XGamingRuntime;
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
#endif

#if PC && !MICROSOFT_GAME_CORE
            return Keyboard.current != null;
#endif
        }

        public static bool IsGamepad()
        {
            return Gamepad.current != null;
        }
    }

    public class UserData
    {
        public static bool IsUserLoggedIn()
        {

#if UWP && !MICROSOFT_GAME_CORE
            return true;
#endif
#if UWP && MICROSOFT_GAME_CORE
            try
            {
                SDK.XBL.XblSocialManagerGetLocalUsers(out XUserHandle[] handles);
                Debug.Log(handles.Length);
                return !(handles.Length == 0);
            }
            catch (Exception e)
            {
               return false;
            }
#endif
#if PC && !MICROSOFT_GAME_CORE
            return SteamClient.IsLoggedOn;
#endif
        }
    }
}
