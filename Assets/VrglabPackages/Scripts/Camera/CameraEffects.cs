using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct CameraShakeData
{
    [Range(0f, 1f)]
    public float Magnitude;
    [Range(0f, 1f)]
    public float Duration;
}

public class CameraEffects : Instancable<CameraEffects>
{
    private Camera cam;

    [Header("Zoom Control")]
    [Range(0f, 1f)] public float ZoomSpeed;
    public int ZoomNormal = 5;
    public int ZoomMax = 10;
    public int ZoomMin = 2;

    [Header("Shake Control")]
    public CameraShakeData shakeData;

    // Private variables
    private Coroutine activeZoomCoroutine;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        ZoomToNormal();
    }

    /// <summary>
    /// Zooms the camera to the specified zoom level.
    /// </summary>
    public void Zoom(int targetZoom)
    {
        if (activeZoomCoroutine != null)
        {
            StopCoroutine(activeZoomCoroutine);
        }
        activeZoomCoroutine = StartCoroutine(ZoomCoroutine(targetZoom));
    }

    /// <summary>
    /// Zooms the camera to the default normal zoom level.
    /// </summary>
    public void ZoomToNormal()
    {
        Zoom(ZoomNormal);
    }

    /// <summary>
    /// Zooms the camera to the maximum zoom level.
    /// </summary>
    public void ZoomToMax()
    {
        Zoom(ZoomMax);
    }

    /// <summary>
    /// Zooms the camera to the minimum zoom level.
    /// </summary>
    public void ZoomToMin()
    {
        Zoom(ZoomMin);
    }

    /// <summary>
    /// Shakes the camera with specified duration and magnitude.
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    /// <summary>
    /// Shakes the camera using pre-defined shake data.
    /// </summary>
    public void Shake()
    {
        Shake(shakeData.Duration, shakeData.Magnitude);
    }

    #region Helper Coroutines

    private IEnumerator ZoomCoroutine(int targetZoom)
    {
        while (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, ZoomSpeed);
            yield return null;
        }
        cam.orthographicSize = targetZoom; // Snap to exact value
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition; // Reset to original position
    }

    #endregion
}
