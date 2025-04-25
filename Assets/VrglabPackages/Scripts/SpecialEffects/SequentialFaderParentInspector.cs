#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SequentialFaderParent))]
public class SequentialFaderParentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SequentialFaderParent lm = (SequentialFaderParent)target;
        if (GUILayout.Button("Fade In"))
        {
            lm.FadeIn();
        }

        if (GUILayout.Button("Fade Out"))
        {
            lm.FadeOut();
        }
    }
}
#endif
