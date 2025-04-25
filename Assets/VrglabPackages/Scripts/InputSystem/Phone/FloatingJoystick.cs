using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FloatingJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI References")]
    public RectTransform joystickBG;
    public RectTransform joystickHandle;

    [Header("Movement")]
    public float handleLimit = 0.5f;

    [Header("Input Manager Override Keys")]
    public string inputXOverrideKey = "pl_mv_x";
    public string inputYOverrideKey = "pl_mv_y";

    [Header("Manual Control")]
    [SerializeField] private bool overrideActive = true;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 joystickCenter = Vector2.zero;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
        joystickCenter = joystickBG.anchoredPosition;
        SetUIVisibility(overrideActive);
    }

    public void SetOverrideActive(bool active)
    {
        overrideActive = active;
        SetUIVisibility(active);

        if (!active)
        {
            ClearOverrides();
        }
    }

    private void SetUIVisibility(bool visible)
    {
        if (joystickBG != null)
            joystickBG.gameObject.SetActive(visible);
        if (joystickHandle != null)
            joystickHandle.gameObject.SetActive(visible);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG, eventData.position, eventData.pressEventCamera, out pos))
        {
            // Normalize to square first
            Vector2 normalized = new Vector2(
                pos.x / (joystickBG.sizeDelta.x * 0.5f),
                pos.y / (joystickBG.sizeDelta.y * 0.5f)
            );

            inputVector = normalized.magnitude > 1 ? normalized.normalized : normalized;

            // Clamp movement to a perfect circle within the smallest axis
            float radius = Mathf.Min(joystickBG.sizeDelta.x, joystickBG.sizeDelta.y) * 0.5f * handleLimit;
            joystickHandle.anchoredPosition = inputVector * radius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (overrideActive)
        {
            if (!string.IsNullOrEmpty(inputXOverrideKey))
                InputManager.Instance.SetManualOverride(inputXOverrideKey, inputVector.x);
            if (!string.IsNullOrEmpty(inputYOverrideKey))
                InputManager.Instance.SetManualOverride(inputYOverrideKey, inputVector.y);
        }
        else
        {
            ClearOverrides();
        }
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
        if (!string.IsNullOrEmpty(inputYOverrideKey))
            InputManager.Instance.ClearManualOverride(inputYOverrideKey);
    }

    public float Horizontal() => inputVector.x;
    public float Vertical() => inputVector.y;
    public Vector2 Direction() => inputVector;
}
