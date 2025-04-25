using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class CameraFollowTarget : Instancable<CameraFollowTarget>
{
    [Header("Movement data")]
    public bool ActivelyFollow = true;
    public float Smoothing = 0.125f;
    public float Speed = 5f;
    public Vector3 Offset;

    [Header("Target data")]
    public Transform Target;

    [Header("Restriction data")]
    public bool FollowY = false;
    public bool BoundsBased = false;
    public bool AutomaticallySetTargetAsPlayer = true;
    public Collider2D BoundsArea;

    [Header("Unity event")]
    public UnityEngine.Events.UnityEvent OnReachedTarget;

    // Private variables
    private bool lookAtObj;
    private Camera mainCamera;

    private void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        if(Target == null && AutomaticallySetTargetAsPlayer)
        {
            try
            {
                Target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            catch (Exception e)
            {

            }
        }
    }

    private void Update()
    {
        if (ActivelyFollow && Target != null)
        {
            Vector3 targetPosition = CalculateTargetPosition();
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, Speed * Time.deltaTime);

            transform.position = smoothedPosition;

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                OnReachedTarget.Invoke();
            }
        }
        else if (lookAtObj)
        {
            MoveToTargetOnce();
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 targetPosition = Target.position + Offset;

        if (!FollowY)
        {
            targetPosition.y = transform.position.y;
        }

        if (BoundsBased && BoundsArea != null)
        {
            // Calculate the half-width and half-height of the camera in world units
            float halfCameraHeight = mainCamera.orthographicSize;
            float halfCameraWidth = halfCameraHeight * mainCamera.aspect;

            // Clamp target position so that the camera view stays within bounds
            targetPosition.x = Mathf.Clamp(targetPosition.x,
                BoundsArea.bounds.min.x + halfCameraWidth,
                BoundsArea.bounds.max.x - halfCameraWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y,
                BoundsArea.bounds.min.y + halfCameraHeight,
                BoundsArea.bounds.max.y - halfCameraHeight);
        }

        targetPosition.z = Offset.z; // Maintain offset for Z

        return targetPosition;
    }

    private void MoveToTargetOnce()
    {
        if (Target != null)
        {
            Vector3 targetPosition = Target.position + Offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Smoothing);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                OnReachedTarget.Invoke();
                lookAtObj = false;
            }
        }
    }

    /// <summary>
    /// Changes the target to the selected GameObject
    /// </summary>
    public void SetTarget(GameObject obj)
    {
        Target = obj.transform;
    }

    /// <summary>
    /// Changes the camera bounds to the selected Collider
    /// </summary>
    public void SetBounds(Collider2D obj)
    {
        BoundsArea = obj;
    }

    /// <summary>
    /// Move's the camera to look at the given GameObject once
    /// </summary>
    public void LookAt(GameObject target)
    {
        Target = target.transform;
        lookAtObj = true;
    }
}
