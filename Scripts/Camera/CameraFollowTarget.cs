using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class CameraFollowTarget : Instancable<CameraFollowTarget>
{
    [Header("Movement data")]
    public bool ActivelyFollow = true;
    public float Smoothing;
    public float speed;
    public Vector3 Offset;

    [Header("Target data")]
    public Transform Target;

    [Header("Restriction data")]
    public bool FollowY = false;
    public bool BoundsBased;
    public Collider2D BoundsArea;

    [Header("Unity event")]
    public UnityEngine.Events.UnityEvent OnReachedTarget;

    //Private variables
    bool LookAtObj;

    private void Update()
    {
        if (ActivelyFollow)
        {
            if (!FollowY)
            {
                if (BoundsBased)
                {
                    float xpos = Mathf.Clamp(Target.position.x + Offset.x, BoundsArea.bounds.min.x, BoundsArea.bounds.max.x);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(xpos, transform.position.y + Offset.y, Offset.z), speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x + Offset.x, transform.position.y + Offset.y, Offset.z), speed * Time.deltaTime);
                }
            }
            else
            {
                if (BoundsBased)
                {
                    float xpos = Mathf.Clamp(Target.position.x + Offset.x, BoundsArea.bounds.min.x, BoundsArea.bounds.max.x);
                    float ypos = Mathf.Clamp(Target.position.y + Offset.y, BoundsArea.bounds.min.y, BoundsArea.bounds.max.y);
                    transform.position = Vector3.Lerp(transform.position, new Vector3(xpos, ypos, Offset.z), speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x + Offset.x * Time.deltaTime, Target.position.y + Offset.y * Time.deltaTime, Offset.z), speed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (LookAtObj)
            {
                if (transform.position != new Vector3(Target.position.x + Offset.x, Target.position.y + Offset.y, transform.position.z + Offset.z))
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x + Offset.x, Target.position.y + Offset.y, Offset.z), speed * Time.deltaTime);
                }
                else
                {
                    OnReachedTarget.Invoke();
                    LookAtObj = false;
                }
            }
        }
    }

    /// <summary>
    /// Changes the target to the selected GameObject
    /// </summary>
    /// <param name="obj">The selected GameObject</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void setTarget(GameObject obj)
    {
        Target = obj.transform;
    }

    /// <summary>
    /// Changes the camera bounds to the selected Collider
    /// </summary>
    /// <param name="obj">The selected Collider</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void setBounds(Collider2D obj)
    {
        BoundsArea = obj;
    }

    /// <summary>
    /// Move's the camera to look at the given GameObject
    /// </summary>
    /// <param name="target">The given GameObject</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void LookAt(GameObject target)
    {
        Target = target.transform;
        LookAtObj = true;
    }
}
