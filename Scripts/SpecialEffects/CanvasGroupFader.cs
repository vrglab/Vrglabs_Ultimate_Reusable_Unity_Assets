using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGroupFader : Fader
{
    [Header("Fade Control")]
    public float fadeSpeed;
    public float fadeTimeSpeed;

    [Header("Specials")]
    public float fadeWaitSpeed;

    private CanvasGroup FadeTXT;
    IEnumerator activeFade;
    IEnumerator specialCase;

    private void Start()
    {
        FadeTXT = GetComponent<CanvasGroup>();
    }

    private IEnumerator FadeToOut()
    {
        while (FadeTXT.alpha > 0)
        {
            FadeTXT.alpha = FadeTXT.alpha - fadeSpeed;
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        FadeTXT.alpha = 0;
        yield return null;
    }

    private IEnumerator FadeToIn()
    {
        while (FadeTXT.alpha < 1)
        {
            FadeTXT.alpha = FadeTXT.alpha + fadeSpeed;
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        FadeTXT.alpha = 1;
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
}
