using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : Instancable<DialogManager>
{
    public Story CurrentStory { get; private set; }

    public bool DialogPlaying { get; private set; }

    /// <summary>
    /// OpensThe dialog system using the given Ink dialog
    /// </summary>
    /// <param name="inkJson">The json file containing the Ink dialog</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void StartDialog(TextAsset inkJson)
    {
        if(!DialogPlaying) 
        {
            CurrentStory = new Story(inkJson.text);
            DialogPlaying = true;
            DialogUIHandler.Instance.OpenDialogPanel();
            ContinueStory();
        }
    }

    /// <summary>
    /// End's a currently running dialog
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void StopDialog()
    {
        DialogUIHandler.Instance.CloseDialogPanel();
        DialogPlaying = false;
    }

    /// <summary>
    /// Advances the currently open dialog either to the next set of text or calls the dialog to be closed if we have reached the end
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void ContinueStory()
    {
        if (CurrentStory.canContinue)
        {
            DialogUIHandler.Instance.SetPrimaryStoryText(LocalizationManager.Instance.GetMultiDataEntry(CurrentStory.Continue()).Property("data").Value.ToString());
            DialogUIHandler.Instance.DisplayChoices(CurrentStory.currentChoices);
        }
        else
        {
            StopDialog();
        }
    }

    /// <summary>
    /// choses a dialog choice to be displayed
    /// </summary>
    /// <param name="index"></param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void ChoseChoice(int index)
    {
        CurrentStory.ChooseChoiceIndex(index);
        ContinueStory();
    }

    public void Start()
    {
        LocalizationManager.Instance.OnLangChanged.AddListener((lang) =>
        {
            DialogUIHandler.Instance.SetPrimaryStoryText(LocalizationManager.Instance.GetMultiDataEntry(CurrentStory.Continue()).Property("data").Value.ToString());
        });
    }

    void Update()
    {
        if(DialogPlaying)
        {
            if (InputManager.Instance.GetKeyDown("dlg_sys_submit"))
            {
                ContinueStory();
            }
        }
    }

}
