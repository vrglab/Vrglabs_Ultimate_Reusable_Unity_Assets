using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public class UIDialogChoice : MonoBehaviour
{
    [Header("Primary data")]
    public int ChoiceIndex;

    [Header("UI")]
    public TextMeshProUGUI txt;

    /// <summary>
    /// Helper class for the ui: it chose's a dialogs system choice
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void ChoiceChosen()
    {
        DialogManager.Instance.ChoseChoice(ChoiceIndex);
    }
}
