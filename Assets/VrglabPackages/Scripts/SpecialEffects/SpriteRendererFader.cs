using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Fades the sprite in or/and out
/// </summary>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SpriteRendererFader : MonoBehaviour
{
    [Header("Fade Control")]
    public SpriteRenderer spriteRenderer;
    public float fadeSpeed;
    public float fadeTimeSpeed;

    [Header("Specials")]
    public float fadeWaitSpeed;

    [Header("Unity events")]
    public UnityEvent OnFullyFadedOut = new UnityEvent();
    public UnityEvent OnFullyFadedIn = new UnityEvent();

    IEnumerator activeFade;
    IEnumerator specialCase;

    #region Helper methods
    private IEnumerator FadeToOut()
    {
        while (spriteRenderer.color.a > 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - fadeSpeed);
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        OnFullyFadedOut.Invoke();
        yield return null;
    }

    private IEnumerator FadeToIn()
    {
        while (spriteRenderer.color.a < 1)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + fadeSpeed);
            yield return new WaitForSeconds(fadeTimeSpeed);
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        OnFullyFadedIn.Invoke();
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
    #endregion

    public void FadeIn()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToIn();
        StartCoroutine(activeFade);
    }

    public void FadeOut()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToOut();
        StartCoroutine(activeFade);
    }

    public void FadeInAndFadeOut()
    {
        if (specialCase != null)
        {
            StopCoroutine(specialCase);
        }
        specialCase = _FadeInAndOut();
        StartCoroutine(specialCase);
    }

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
