using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingTextAnimation : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_TextMeshPro;
    public float waitTime;
    public string loadingTextEntryID = "loading";

    public bool IsPlaying { get; internal set; } = false;
    IEnumerator coroutine;

    private IEnumerator anim()
    {
        var text = LocalizationManager.Instance.GetEntry(loadingTextEntryID);
        while (true)
        {
            if (text == LocalizationManager.Instance.GetEntry(loadingTextEntryID) + "...")
            {
                text = LocalizationManager.Instance.GetEntry(loadingTextEntryID);
            }
            else
            {
                text += ".";
            }
            m_TextMeshPro.text = text;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void Start()
    {
        LocalizationManager.Instance.OnLangChanged.AddListener((LangID) =>
        {
            if (IsPlaying)
            {
                Stop();
                Play();
            }
        });
    }

    /// <summary>
    /// Play's the hardcoded animation
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Play()
    {
        coroutine = anim();
        IsPlaying = true;
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// Stop's the hardcoded animation
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Stop()
    {
        if (coroutine != null)
        {
            IsPlaying = false;
            StopCoroutine(coroutine);
        }
    }
}