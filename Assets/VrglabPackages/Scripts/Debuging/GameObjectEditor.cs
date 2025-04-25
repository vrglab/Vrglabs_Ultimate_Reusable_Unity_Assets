using UnityEngine;
using System;
using System.Reflection;

public class GameObjectEditor : Instancable<GameObjectEditor>
{
#if DEBUG
    public float referenceWidth = 1920f, referenceHeight = 1080f;
    private Rect windowRect = new Rect(520, 10, 800, 400);
    private Vector2 scrollPosition;



    void OnGUI()
    {
        if(HoverInfo.Instance.showInfoWindow)
        {
            float scaleX = Screen.width / referenceWidth;
            float scaleY = Screen.height / referenceHeight;

            // Apply scaling
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1));

            // Save the original matrix
            Matrix4x4 originalMatrix = GUI.matrix;


            HoverInfo.Instance.isMouseOverEditGUI = windowRect.Contains(Event.current.mousePosition);
            windowRect = GUI.Window(2, windowRect, EditWindow, "Edit GameObject");
            GUI.matrix = GUI.matrix = originalMatrix;
        }
    }

    void EditWindow(int windowID)
    {
        GUILayout.BeginVertical();

        // Add a close button
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add a scroll view for the editor
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowRect.width - 20), GUILayout.Height(windowRect.height - 50));

        GameObject selectedObject = HoverInfo.Instance.selectedObject;
        if (selectedObject != null)
        {
            // Display and edit position
            Vector3 position = selectedObject.transform.position;
            GUILayout.Label("Position");
            position.x = FloatField("X", position.x);
            position.y = FloatField("Y", position.y);
            position.z = FloatField("Z", position.z);
            selectedObject.transform.position = position;

            // Display and edit rotation
            Vector3 rotation = selectedObject.transform.rotation.eulerAngles;
            GUILayout.Label("Rotation");
            rotation.x = FloatField("X", rotation.x);
            rotation.y = FloatField("Y", rotation.y);
            rotation.z = FloatField("Z", rotation.z);
            selectedObject.transform.rotation = Quaternion.Euler(rotation);

            // Display and edit scale
            Vector3 scale = selectedObject.transform.localScale;
            GUILayout.Label("Scale");
            scale.x = FloatField("X", scale.x);
            scale.y = FloatField("Y", scale.y);
            scale.z = FloatField("Z", scale.z);
            selectedObject.transform.localScale = scale;

            // Display and edit components
            Component[] components = selectedObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                GUILayout.Label(component.GetType().Name);
                EditComponent(component);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(windowRect.width - 43)))
            {
                MonoBehaviour.Destroy(HoverInfo.Instance.selectedObject);
                HoverInfo.Instance.selectedObject = null;
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        // Make the window draggable
        GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
    }

    float FloatField(string label, float value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(20));
        string valueStr = GUILayout.TextField(value.ToString());
        float.TryParse(valueStr, out value);
        GUILayout.EndHorizontal();
        return value;
    }

    void EditComponent(Component component)
    {
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(float))
            {
                float value = (float)field.GetValue(component);
                value = FloatField(field.Name, value);
                field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(int))
            {
                int value = (int)field.GetValue(component);
                value = IntField(field.Name, value);
                field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(string))
            {
                string value = (string)field.GetValue(component);
                value = StringField(field.Name, value);
                field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(bool))
            {
                bool value = (bool)field.GetValue(component);
                value = BoolField(field.Name, value);
                field.SetValue(component, value);
            }
            // Add more field types as needed
        }
    }

    int IntField(string label, int value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        string valueStr = GUILayout.TextField(value.ToString());
        int.TryParse(valueStr, out value);
        GUILayout.EndHorizontal();
        return value;
    }

    string StringField(string label, string value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        value = GUILayout.TextField(value);
        GUILayout.EndHorizontal();
        return value;
    }

    bool BoolField(string label, bool value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        value = GUILayout.Toggle(value, "");
        GUILayout.EndHorizontal();
        return value;
    }
#endif
}