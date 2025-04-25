using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class UIFader : Fader
{
    [Header("Fade Control")]
    public float fadeSpeed;
    public float fadeTimeSpeed;

    [Header("Specials")]
    public float fadeWaitSpeed;

    private MaskableGraphic FadeTXT;
    IEnumerator activeFade;
    IEnumerator specialCase;

    private void Start()
    {
        FadeTXT = GetComponent<MaskableGraphic>();
    }

    private IEnumerator FadeToOut()
    {
        while (FadeTXT.color.a > 0)
        {
            FadeTXT.color = new Color(FadeTXT.color.r, FadeTXT.color.g, FadeTXT.color.b, FadeTXT.color.a - fadeSpeed);
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        FadeTXT.color = new Color(FadeTXT.color.r, FadeTXT.color.g, FadeTXT.color.b, 0);
        yield return null;
    }

    private IEnumerator FadeToIn()
    {
        while (FadeTXT.color.a < 1)
        {
            FadeTXT.color = new Color(FadeTXT.color.r, FadeTXT.color.g, FadeTXT.color.b, FadeTXT.color.a + fadeSpeed);
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        FadeTXT.color = new Color(FadeTXT.color.r, FadeTXT.color.g, FadeTXT.color.b, 1);
        yield return null;
    }

    private IEnumerator _FadeInAndOut()
    {
        FadeIn();
        yield return new WaitForSeconds(fadeWaitSpeed);
        FadeOut();
    }

    private IEnumerator _FadeOutAndIn()
    {
        FadeOut();
        yield return new WaitForSeconds(fadeWaitSpeed);
        FadeIn();
    }

    
    public override void FadeIn()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToIn();
        StartCoroutine(activeFade);
    }

    public override void FadeOut()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToOut();
        StartCoroutine(activeFade);
    }

    /// <summary>
    /// Fades the text in and then out after some time
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void FadeInAndFadeOut()
    {
        if (specialCase != null)
        {
            StopCoroutine(specialCase);
        }
        specialCase = _FadeInAndOut();
        StartCoroutine(specialCase);
    }

    /// <summary>
    /// fades the text out and then in after some time
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void FadeOutAndFadeIn()
    {
        if (specialCase != null)
        {
            StopCoroutine(specialCase);
        }
        specialCase = _FadeOutAndIn();
        StartCoroutine(specialCase);
    }

    public override float FadeSpeed()
    {
        return fadeSpeed;
    }
}

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public abstract class Fader : MonoBehaviour
{
    /// <summary>
    /// Fades the UiGraphic in
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public abstract void FadeIn();
    /// <summary>
    /// Fades the UiGraphic out
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public abstract void FadeOut();

    public abstract float FadeSpeed();
}
