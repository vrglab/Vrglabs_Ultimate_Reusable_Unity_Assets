using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trigger activates when the player enters or exits from specified directions, with different actions based on direction.
/// </summary>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Trigger : MonoBehaviour
{
    public enum TriggerDirection { Any, FromAbove, FromBelow, FromLeft, FromRight }

    [Header("Settings")]
    public bool Constant = false;  // True: trigger multiple times; False: trigger once for entry and exit
    public bool Collidable = false;

    [Header("Unity Events for Enter Directions")]
    public UnityEvent OnEnterFromAny;
    public UnityEvent OnEnterFromAbove;
    public UnityEvent OnEnterFromBelow;
    public UnityEvent OnEnterFromLeft;
    public UnityEvent OnEnterFromRight;

    [Header("Unity Events for Exit Directions")]
    public UnityEvent OnExitFromAny;
    public UnityEvent OnExitFromAbove;
    public UnityEvent OnExitFromBelow;
    public UnityEvent OnExitFromLeft;
    public UnityEvent OnExitFromRight;

    private Collider2D col;
    private bool hasEntered = false; // Track if entry has been triggered once
    private bool hasExited = false;  // Track if exit has been triggered once

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        col = GetComponent<Collider2D>();
        col.isTrigger = !Collidable;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (Constant || !hasEntered))
        {
            TriggerDirection entryDirection = GetDirection(collision);
            OnEnterFromAny.Invoke();  // Always invoke the "any" event
            InvokeEnterEvent(entryDirection);  // Invoke specific direction event
            hasEntered = true;  // Mark entry as triggered if Constant is false
            if (Constant) hasExited = false; // Reset exit gating for future exits if Constant is false
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (Constant || (!hasExited && hasEntered)))
        {
            TriggerDirection exitDirection = GetDirection(collision);
            OnExitFromAny.Invoke();  // Always invoke the "any" event
            InvokeExitEvent(exitDirection);  // Invoke specific direction event
            hasExited = true;  // Mark exit as triggered if Constant is false
            if (Constant) hasEntered = false; // Reset entry gating for future entries if Constant is false
        }
    }

    private void InvokeEnterEvent(TriggerDirection direction)
    {
        switch (direction)
        {
            case TriggerDirection.FromAbove:
                OnEnterFromAbove.Invoke();
                break;
            case TriggerDirection.FromBelow:
                OnEnterFromBelow.Invoke();
                break;
            case TriggerDirection.FromLeft:
                OnEnterFromLeft.Invoke();
                break;
            case TriggerDirection.FromRight:
                OnEnterFromRight.Invoke();
                break;
            case TriggerDirection.Any:
            default:
                break;
        }
    }

    private void InvokeExitEvent(TriggerDirection direction)
    {
        switch (direction)
        {
            case TriggerDirection.FromAbove:
                OnExitFromAbove.Invoke();
                break;
            case TriggerDirection.FromBelow:
                OnExitFromBelow.Invoke();
                break;
            case TriggerDirection.FromLeft:
                OnExitFromLeft.Invoke();
                break;
            case TriggerDirection.FromRight:
                OnExitFromRight.Invoke();
                break;
            case TriggerDirection.Any:
            default:
                break;
        }
    }

    private TriggerDirection GetDirection(Collider2D collision)
    {
        Bounds bounds = col.bounds;
        Vector3 playerPosition = collision.transform.position;

        // Calculate distances to each edge of the bounds
        float distanceFromTop = bounds.max.y - playerPosition.y;
        float distanceFromBottom = playerPosition.y - bounds.min.y;
        float distanceFromLeft = playerPosition.x - bounds.min.x;
        float distanceFromRight = bounds.max.x - playerPosition.x;

        // Find the smallest distance to determine the entry direction
        float minDistance = Mathf.Min(distanceFromTop, distanceFromBottom, distanceFromLeft, distanceFromRight);

        if (minDistance == distanceFromTop)
            return TriggerDirection.FromAbove;
        else if (minDistance == distanceFromBottom)
            return TriggerDirection.FromBelow;
        else if (minDistance == distanceFromLeft)
            return TriggerDirection.FromLeft;
        else
            return TriggerDirection.FromRight;
    }
}
