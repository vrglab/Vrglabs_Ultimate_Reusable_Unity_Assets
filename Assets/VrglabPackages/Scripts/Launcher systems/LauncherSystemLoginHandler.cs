using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using conditions.json;
using UnityEngine.Events;

#if UWP || MICROSOFT_GAME_CORE && !UNITY_ANDROID && !UNITY_IPHONE
using Unity.XGamingRuntime;
#endif

#if PC && !MICROSOFT_GAME_CORE && !UNITY_ANDROID && !UNITY_IPHONE
using Steamworks;
#endif

public class LauncherSystemLoginHandler : MonoBehaviour
{
    private InputAction anyButtonAction;

    [SerializeField]
    private UnityEvent UserLogedIn;

    void Start()
    {
        anyButtonAction = new InputAction("AnyInput", InputActionType.PassThrough);
        anyButtonAction.AddBinding("<Keyboard>/anyKey");
        anyButtonAction.AddBinding("<Gamepad>/*");
        anyButtonAction.AddBinding("<Touchscreen>/primaryTouch/press");
        anyButtonAction.AddBinding("<Pointer>/press"); // For mouse or simulated touch
        anyButtonAction.performed += OnAnyButtonPress;
        anyButtonAction.Enable();

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
        if(!UserData.IsUserLoggedIn())
        {
#if PC && !MICROSOFT_GAME_CORE && !UNITY_ANDROID && !UNITY_IPHONE

#elif UWP || MICROSOFT_GAME_CORE && !UNITY_ANDROID && !UNITY_IPHONE
            GDK.Instance.LoginUser();
            UserLogedIn.Invoke();
#elif GAMEJOLT && !UNITY_ANDROID && !UNITY_IPHONE
            GameJolt.UI.GameJoltUI.Instance.ShowSignIn((signedin) =>
            {
                if(signedin)
                {
                    UserLogedIn.Invoke();
                }
            });
#elif UNITY_ANDROID || UNITY_IPHONE
            UserLogedIn.Invoke();
#endif
        }
        else
        {
            UserLogedIn.Invoke();
        }
        anyButtonAction.performed -= OnAnyButtonPress;
    }
}
