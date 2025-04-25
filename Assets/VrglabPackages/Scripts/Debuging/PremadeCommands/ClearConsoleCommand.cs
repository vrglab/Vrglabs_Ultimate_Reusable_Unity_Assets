using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Command("clear", 0)]
[HelpCommandData("Clears the console", new string[] {  })]
public class ClearConsoleCommand : ICommand
{
    public void Execute(string[] args, KeyValuePair<string, string>[] options)
    {
#if DEBUG
        DevConsole.Instance.ClearConsole();
#endif
    }
}
