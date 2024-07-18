using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Command("open-window", 1)]
[HelpCommandData("Open's the given window based on it's name", new string[] { "window name"})]
public class OpenWindow : ICommand
{
    public void Execute(string[] args, KeyValuePair<string, string>[] options)
    {
        switch (args[0])
        {
            case "InfoControl":
#if DEBUG
                HoverInfo.Instance.showInfoWindow = true;
#endif
                break;
        }
    }
}
