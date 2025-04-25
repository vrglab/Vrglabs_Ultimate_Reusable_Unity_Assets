using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevConsole : Instancable<DevConsole>
{
#if DEBUG
    public float referenceWidth = 1920f;
    public float referenceHeight = 1080f;

    public bool active;

    Rect windowRect = new Rect(10, 320, 500, 300);

    private string input = "";
    private Vector2 scrollPosition;
    private List<LogEntry> logs = new List<LogEntry>();

    private class LogEntry
    {
        public string message;
        public LogType type;
        public bool user;

        public LogEntry(string message, LogType type, bool user = false)
        {
            this.message = message;
            this.type = type;
            this.user = user;
        }
    }


    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logs.Add(new LogEntry(logString, type));
    }

    void OnGUI()
    {
        if (active)
        {
            HoverInfo.Instance.showInfoWindow = true;
            float scaleX = Screen.width / referenceWidth;
            float scaleY = Screen.height / referenceHeight;

            // Apply scaling
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1));

            DisableRichTextForAllStyles(GUI.skin);
            // Save the original matrix
            Matrix4x4 originalMatrix = GUI.matrix;

            HoverInfo.Instance.isMouseOverConsoleGUI = windowRect.Contains(Event.current.mousePosition);
            windowRect = GUI.Window(0, windowRect, ConsoleWindow, "Developer Console");
            GUI.matrix = originalMatrix;
        }
        else 
        {
            HoverInfo.Instance.showInfoWindow = false;
        }
    }

    void ConsoleWindow(int windowID)
    {
        GUILayout.BeginVertical();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(windowRect.width - 20), GUILayout.Height(windowRect.height - 70));

        foreach (LogEntry log in logs)
        {
            if(!log.user)
            {
                GUI.color = GetLogColor(log.type);
            }
            else
            {
                GUI.color = Color.blue;
            }
            GUILayout.Label(log.message);
        }

        GUI.color = Color.white;
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        input = GUILayout.TextField(input, GUILayout.Width(windowRect.width - 70));
        if (GUILayout.Button("Enter", GUILayout.Width(50)))
        {
            logs.Add(new LogEntry(input, LogType.Log, true));
            CommandManager.ProcessCommands(input);
            input = "";
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
    }

    public void Update()
    {
        if(false) 
        {
            active = !active;

            if(active)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return Color.red;
            case LogType.Warning:
                return Color.yellow;
            case LogType.Assert:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void DisableRichTextForAllStyles(GUISkin skin)
    {
        foreach (GUIStyle style in skin.customStyles)
        {
            style.richText = false;
        }

        skin.label.richText = false;
        skin.box.richText = false;
        skin.button.richText = false;
        skin.toggle.richText = false;
        skin.window.richText = false;
        skin.textField.richText = false;
        skin.textArea.richText = false;
        skin.horizontalSlider.richText = false;
        skin.horizontalSliderThumb.richText = false;
        skin.verticalSlider.richText = false;
        skin.verticalSliderThumb.richText = false;
        skin.horizontalScrollbar.richText = false;
        skin.horizontalScrollbarThumb.richText = false;
        skin.horizontalScrollbarLeftButton.richText = false;
        skin.horizontalScrollbarRightButton.richText = false;
        skin.verticalScrollbar.richText = false;
        skin.verticalScrollbarThumb.richText = false;
        skin.verticalScrollbarUpButton.richText = false;
        skin.verticalScrollbarDownButton.richText = false;
        skin.scrollView.richText = false;
    }

    public void ClearConsole()
    {
        logs.Clear();
    }

    public void Log(object message, LogType type = LogType.Log, bool user = false)
    {
        logs.Add(new LogEntry(message.ToString(), type, user));
    }
#endif
}
