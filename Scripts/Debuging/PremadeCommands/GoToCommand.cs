using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Command("goto", 1, "b")]
[HelpCommandData("Loads a given scene", new string[] { "Scene to load" }, validOptionNames = new string[] { "b"})]
public class GoToCommand : ICommand
{
    public void Execute(string[] args, KeyValuePair<string, string>[] options)
    {
        if(options.Contains("b"))
        {
            Debug.Log($"Loading Scene with index {args[0]}");
            SceneManager.LoadScene(int.Parse(args[0]));
        } else
        {
            Debug.Log($"Loading Scene with name {args[0]}");
            SceneManager.LoadScene(args[0]);
        }
    }
}
