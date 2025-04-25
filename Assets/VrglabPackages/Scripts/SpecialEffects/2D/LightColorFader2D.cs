using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static System.TimeZoneInfo;

/// <summary>
/// Fades the color of light to whatever is chosen
/// </summary>
public class LightColorFader2D : Fader
{
    [Header("Fade Control")]
    public Light2D LightSource;
    public Color color1;
    public Color color2;

    public float fadeSpeed;
    public float TotalfadeTime;


    IEnumerator activeFade;

    private IEnumerator CalculateColor(Color color)
    {


        float timeElapsed = 0f;
        float totalTime = TotalfadeTime;

        Color startColor = LightSource.color;
        Color endColor = color;

        while (timeElapsed < totalTime)
        {
            timeElapsed += Time.deltaTime;
            LightSource.color = Color.Lerp(startColor, endColor, timeElapsed / totalTime);
            yield return new WaitForSeconds(fadeSpeed);
        }

        LightSource.color = color;
        yield return null;
    }


    public void FadeToColor1()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = CalculateColor(color1);
        StartCoroutine(activeFade);
    }

    public void FadeToColor2()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = CalculateColor(color2);
        StartCoroutine(activeFade);
    }

    public override void FadeIn()
    {
        FadeToColor1();
    }

    public override void FadeOut()
    {
        FadeToColor2();
    }

    public override float FadeSpeed()
    {
        return fadeSpeed;
    }
}
