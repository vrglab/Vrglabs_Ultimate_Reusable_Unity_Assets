using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SequentialFaderChild : MonoBehaviour
{
    public SequentialFaderParent parent;
    private MaskableGraphic graphic;
    IEnumerator activeFade;
    IEnumerator specialCase;

    private void Start()
    {
        graphic = GetComponent<MaskableGraphic>();
    }

    private IEnumerator FadeToOut()
    {
        while (graphic.color.a > 0)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a - parent.fadeSpeed);
            yield return new WaitForSeconds(parent.fadeTimeSpeed);
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);
        yield return null;
    }

    private IEnumerator FadeToIn()
    {
        while (graphic.color.a < 1)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a + parent.fadeSpeed);
            yield return new WaitForSeconds(parent.fadeTimeSpeed);
        }
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1);
        yield return null;
    }

    private IEnumerator _FadeInAndOut()
    {
        FadeIn();
        yield return new WaitForSeconds(parent.fadeWaitSpeed);
        FadeOut();
    }

    private IEnumerator _FadeOutAndIn()
    {
        FadeOut();
        yield return new WaitForSeconds(parent.fadeWaitSpeed);
        FadeIn();
    }

    /// <summary>
    /// Fades the text in
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void FadeIn()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToIn();
        StartCoroutine(activeFade);
    }

    /// <summary>
    /// Fades the text out
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void FadeOut()
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        activeFade = FadeToOut();
        StartCoroutine(activeFade);
    }
}
