using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using conditions.json;

#if UWP || MICROSOFT_GAME_CORE
using Microsoft.Xbox;
using XGamingRuntime;
#endif

#if PC && !MICROSOFT_GAME_CORE
using Steamworks;
#endif

public class LauncherSystemLoginHandler : MonoBehaviour
{
    private InputAction anyButtonAction;

    void Start()
    {
        anyButtonAction = new InputAction(binding: InputManager.Instance.ActiveInputDevice.path + "/*");
        anyButtonAction.performed += OnAnyButtonPress;
        anyButtonAction.Enable();
    }

    void OnDisable()
    {
        anyButtonAction.performed -= OnAnyButtonPress;
        anyButtonAction.Disable();
    }

    private void OnAnyButtonPress(InputAction.CallbackContext context)
    {

        if(UserData.IsUserLoggedIn())
        {
            LoadingController.Instance.LoadTheScene("MainMenu");
        }
        else
        {
#if UWP && !MICROSOFT_GAME_CORE
            LoadingController.Instance.LoadTheScene("MainMenu");
#endif
#if UWP && MICROSOFT_GAME_CORE
            Gdk.Helpers.SignIn();
#endif

#if PC && !MICROSOFT_GAME_CORE
            LoadingController.Instance.LoadTheScene("MainMenu");
#endif
        }
    }
}
