using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogUIHandler : Instancable<DialogUIHandler>
{
    [Header("Default Choice object")]
    [SerializeField] private GameObject ChoiceOBJ;

    [Header("UI")]
    public float letterPause = 0.2f;

    [SerializeField] private GameObject DialogSystemUI;
    [SerializeField] private GameObject DialogSystemChoicesPanel;
    [SerializeField] private TextMeshProUGUI PrimaryTextField;


    private List<GameObject> Created_ui_Choices = new List<GameObject>();

    IEnumerator running_txt;


    public void OpenDialogPanel()
    {
        DialogSystemUI.SetActive(true);
    }

    public void CloseDialogPanel()
    {
        DialogSystemUI.SetActive(false);
        PrimaryTextField.text = "";
    }

    public void SetPrimaryStoryText(string data)
    {
        if(running_txt != null)
        {
            StopCoroutine(running_txt);
        }
        running_txt = _setPrimaryStoryText(data);
        StartCoroutine(running_txt);
    }

    public void DisplayChoices(List<Choice> choices)
    {
        foreach (var ui_ellemnt in Created_ui_Choices)
        {
            DestroyImmediate(ui_ellemnt.gameObject);
        }
        Created_ui_Choices.Clear();
        for (int i = 0;i < choices.Count;i++)
        {
            Choice choice = choices[i];

            GameObject instance = Instantiate(ChoiceOBJ);
            instance.transform.SetParent(DialogSystemChoicesPanel.transform);
            UIDialogChoice uIDialogChoice = instance.GetComponent<UIDialogChoice>();
            uIDialogChoice.txt.text = LocalizationManager.Instance.GetMultiDataEntry(choice.text).Property("name").Value.ToString();
            uIDialogChoice.ChoiceIndex = choice.index;

            LocalizationManager.Instance.OnLangChanged.AddListener((lang) =>
            {
                uIDialogChoice.txt.text = LocalizationManager.Instance.GetMultiDataEntry(choice.text).Property("name").Value.ToString();
            });

            Created_ui_Choices.Add(instance);
            instance.SetActive(true);
        }
    }

    private IEnumerator _setPrimaryStoryText(string data)
    {
        PrimaryTextField.text = "";
        foreach (char letter in data.ToCharArray())
        {
            PrimaryTextField.text += letter;
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
        running_txt = null;
    }
}
