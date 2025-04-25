using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TMPLocalized : MonoBehaviour
{
    public bool LoadOnStart = true;
    public string EntryId;
    private TMPro.TextMeshProUGUI tmp;

    public UnityEvent OnReset = new UnityEvent();

    private bool AlreadyLoaded;

    void Start()
    {
        tmp = GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp != null)
        {
            if (LoadOnStart)
            {
                ResetText();
            }
            LocalizationManager.Instance.OnLangChanged.AddListener((lang) =>
            {
                if (AlreadyLoaded)
                {
                    ResetText();
                }
            });
        }
    }

    /// <summary>
    /// Sets the entry id to whatever is given to us
    /// </summary>
    /// <param name="entryID">The requested string to set entry id to</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void SetEntryID(string entryID)
    {
        this.EntryId = entryID;
    }


    /// <summary>
    /// Reset's the text to whatever is In our EntryID
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void ResetText()
    {
        OnReset.Invoke();
        tmp.text = StringProccessor.Instance.GetEntry(EntryId, gameObject);
        AlreadyLoaded = true;
    }
}