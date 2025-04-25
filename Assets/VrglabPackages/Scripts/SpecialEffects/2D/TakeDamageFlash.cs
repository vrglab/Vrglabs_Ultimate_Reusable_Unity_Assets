using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class TakeDamageFlash : MonoBehaviour
{
    [Header("Fade Control")]
    public Light2D LightSource;
    public Color Color;

    public float FlashSpeed, TotalflashTime;


    private bool canFlash;

    private void Start()
    {
        canFlash = true;
    }

    public void FlashScreen()
    {
        if (canFlash)
        {
            StartCoroutine(CalculateColor());
        }
    }

    private IEnumerator CalculateColor()
    {
        canFlash = false;
        float timeElapsed = 0f;
        float totalTime = TotalflashTime;

        Color endColor = Color, startColor = LightSource.color; ;

        while (timeElapsed < totalTime)
        {
            timeElapsed += Time.deltaTime;
            LightSource.color = Color.Lerp(startColor, endColor, timeElapsed / totalTime);
            yield return new WaitForSeconds(FlashSpeed);
        }
        timeElapsed = 0f;
        while (timeElapsed < totalTime)
        {
            timeElapsed += Time.deltaTime;
            LightSource.color = Color.Lerp(endColor, startColor, timeElapsed / totalTime);
            yield return new WaitForSeconds(FlashSpeed);
        }
        LightSource.color = startColor;
        canFlash = true;
        yield return null;
    }
}
