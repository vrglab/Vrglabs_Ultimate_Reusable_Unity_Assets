using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SequentialFaderParent : Fader
{
    [Header("Fade Control")]
    public float fadeSpeed;
    public float fadeTimeSpeed;
    [Header("Specials")]
    public float fadeWaitSpeed;

    public UnityEvent OnFadeIn = new UnityEvent();
    public UnityEvent OnFadeOut = new UnityEvent();

    List<GameObject> children = new List<GameObject>();

    private void Start()
    {
        foreach (var item in GetComponentsInChildren<SequentialFaderChild>())
        {
            item.gameObject.GetComponent<SequentialFaderChild>().parent= this;
            children.Add(item.gameObject);
        }  
    }


    public override void FadeIn()
    {
        OnFadeIn.Invoke();
        foreach (var item in children)
        {
            item.GetComponent<SequentialFaderChild>().FadeIn();
        }
    }

    public override void FadeOut()
    {
        OnFadeOut.Invoke();
        foreach (var item in children)
        {
            item.GetComponent<SequentialFaderChild>().FadeOut();
        }
    }

    public override float FadeSpeed()
    {
        return fadeSpeed;
    }
}
