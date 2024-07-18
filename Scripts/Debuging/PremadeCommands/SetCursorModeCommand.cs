using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Command("set-cursor-mode", 1)]
[HelpCommandData("Set's the current cursor mode", new string[] { "Locked | Lose" })]
public class SetCursorModeCommand : ICommand
{
    public void Execute(string[] args, KeyValuePair<string, string>[] options)
    {
        switch (args[0])
        {
            case "locked":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Set cursor mode to: locked");
                break;
            case "lose":
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("Set cursor mode to: lose");
                break;
        }
    }
}
