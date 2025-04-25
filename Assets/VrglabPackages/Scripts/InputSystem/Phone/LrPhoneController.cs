using UnityEngine;

public class LrPhoneController : MonoBehaviour
{

    [Header("Input Manager Override Keys")]
    public string inputXOverrideKey = "pl_mv_x";
    public string inputJumpOverrideKey = "pl_mv_jump";


    public void Left()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!string.IsNullOrEmpty(inputXOverrideKey))
            InputManager.Instance.SetManualOverride(inputXOverrideKey, 1f);
#else
        ClearOverrides();
#endif
    }

    public void Right()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!string.IsNullOrEmpty(inputXOverrideKey))
            InputManager.Instance.SetManualOverride(inputXOverrideKey, -1f);
#else
        ClearOverrides();
#endif
    }


    public void Jump()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!string.IsNullOrEmpty(inputJumpOverrideKey))
            InputManager.Instance.SetManualOverride(inputJumpOverrideKey, true);
#else
        ClearOverrides();
#endif
    }

    public void ResetJumpInput()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!string.IsNullOrEmpty(inputJumpOverrideKey))
            InputManager.Instance.SetManualOverride(inputJumpOverrideKey, false);
#else
        ClearOverrides();
#endif
    }


    public void ResetInput()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!string.IsNullOrEmpty(inputXOverrideKey))
            InputManager.Instance.SetManualOverride(inputXOverrideKey, 0f);
#else
        ClearOverrides();
#endif
    }



    private void OnDisable()
    {
        ClearOverrides();
    }


    private void ClearOverrides()
    {
        if (!string.IsNullOrEmpty(inputXOverrideKey))
            InputManager.Instance.ClearManualOverride(inputXOverrideKey);
    }
}
