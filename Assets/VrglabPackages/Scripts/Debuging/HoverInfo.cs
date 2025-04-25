using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HoverInfo : Instancable<HoverInfo>
{
#if DEBUG
    public GameObject hoveredObject, selectedObject = null;
    private Component selectedComponent = null;
    private string objectInfo = "",  componentInfo = "";
    private Vector2 scrollPosition;
    private Rect windowRect = new Rect(10, 10, 500, 300);
    public bool isMouseOverGUI = false, isMouseOverEditGUI, isMouseOverConsoleGUI;
    private bool isResizing = false;
    private Vector2 resizeStartPos;

    public bool showInfoWindow = true;

    public float referenceWidth = 1920f, referenceHeight = 1080f;

    private bool isDragging = false;
    private Vector3 offset;

    void Update()
    {
        if (!isMouseOverGUI && !isMouseOverEditGUI && !isMouseOverConsoleGUI && showInfoWindow)
        {
            HandleMouseInput();
        }

        if (isDragging && selectedObject != null && showInfoWindow)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = selectedObject.transform.position.z; // Keep original z position
            selectedObject.transform.position = mousePosition + offset;

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }

    void HandleMouseInput()
    {
        // Create a ray from the mouse cursor position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            hoveredObject = hit.collider.gameObject;

            if (selectedObject == null)
            {
                objectInfo = GetGameObjectInfo(hoveredObject);
            }

            if (false)
            {
                selectedObject = hoveredObject;
                objectInfo = GetGameObjectInfo(selectedObject);
                componentInfo = "";

                // Calculate the offset
                offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset.z = 0; // Keep offset in 2D plane
                isDragging = true;
            }
        }
        else
        {
            hoveredObject = null;
            if (selectedObject == null)
            {
                objectInfo = "No GameObject under mouse.";
            }
        }

        // Deselect object if clicked on empty space
        if (false && hoveredObject == null)
        {
            selectedObject = null;
            objectInfo = "No GameObject under mouse.";
            componentInfo = "";
        }
    }


    void InfoWindow(int windowID)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
        {
            showInfoWindow = false;
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowRect.width - 20), GUILayout.Height(windowRect.height - 90));
        GUILayout.Label(objectInfo);

        if (selectedObject != null)
        {
            Component[] components = selectedObject.GetComponents<Component>();
            GUILayout.Label("Components:");
            foreach (Component component in components)
            {
                if (GUILayout.Button(component.GetType().Name, GUILayout.Width(windowRect.width - 43)))
                {
                    selectedComponent = component;
                    componentInfo = GetComponentInfo(selectedComponent);
                }
            }
        }

        if (!string.IsNullOrEmpty(componentInfo))
        {
            GUILayout.Label("Component Info:");
            GUILayout.Label(componentInfo);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
    }

    private string GetGameObjectInfo(GameObject obj)
    {
        if (obj == null)
        {
            return "No GameObject under mouse.";
        }

        // Collect information about the GameObject
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Name: {obj.name}");
        sb.AppendLine($"Tag: {obj.tag}");
        sb.AppendLine($"Layer: {LayerMask.LayerToName(obj.layer)}");
        sb.AppendLine($"Active: {obj.activeSelf}");
        sb.AppendLine($"Position: {obj.transform.position}");
        sb.AppendLine($"Rotation: {obj.transform.rotation.eulerAngles}");
        sb.AppendLine($"Scale: {obj.transform.localScale}");

        if (obj.transform.parent != null)
        {
            sb.AppendLine($"Parent: {obj.transform.parent.name}");
        }
        else
        {
            sb.AppendLine("Parent: None");
        }

        int childCount = obj.transform.childCount;
        sb.AppendLine($"Number of Children: {childCount}");
        if (childCount > 0)
        {
            sb.AppendLine("Children:");
            for (int i = 0; i < childCount; i++)
            {
                sb.AppendLine($"- {obj.transform.GetChild(i).name}");
            }
        }

        return sb.ToString();
    }

    private string GetComponentInfo(Component component)
    {
        if (component == null)
        {
            return "";
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"Component: {component.GetType().Name}");

        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        sb.AppendLine("Fields:");
        foreach (FieldInfo field in fields)
        {
            try
            {
                sb.AppendLine($"{field.Name} ({field.FieldType.Name}): {field.GetValue(component)}");
            }
            catch
            {
                sb.AppendLine($"{field.Name} ({field.FieldType.Name}): Could not retrieve value");
            }
        }

        sb.AppendLine("Properties:");
        foreach (PropertyInfo property in properties)
        {
            if (property.CanRead)
            {
                try
                {
                    sb.AppendLine($"{property.Name} ({property.PropertyType.Name}): {property.GetValue(component, null)}");
                }
                catch
                {
                    sb.AppendLine($"{property.Name} ({property.PropertyType.Name}): Could not retrieve value");
                }
            }
        }

        return sb.ToString();
    }

    void OnGUI()
    {
        if(showInfoWindow)
        {
            float scaleX = Screen.width / referenceWidth;
            float scaleY = Screen.height / referenceHeight;

            // Apply scaling
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1));

            // Save the original matrix
            Matrix4x4 originalMatrix = GUI.matrix;


            isMouseOverGUI = windowRect.Contains(Event.current.mousePosition);
            windowRect = GUI.Window(1, windowRect, InfoWindow, "Hovered GameObject Info");

            GUI.matrix = originalMatrix;
        }
    }
#endif
}
