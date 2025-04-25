#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationManager))]
public class LocalizationManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LocalizationManager lm = (LocalizationManager)target;
        if (GUILayout.Button("Change lang To Selected"))
        {
            lm.ChangeLang(lm.LangID);
        }
    }
}

#endif
