using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    public UnityEvent OnHoldStart;
    [SerializeField]
    public UnityEvent OnHold;
    [SerializeField]
    public UnityEvent OnHoldEnd;

    private bool isHeld = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        OnHoldStart.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        OnHoldEnd.Invoke();
    }

    void Update()
    {
        if (isHeld)
        {
            OnHold.Invoke();
        }
    }
}
