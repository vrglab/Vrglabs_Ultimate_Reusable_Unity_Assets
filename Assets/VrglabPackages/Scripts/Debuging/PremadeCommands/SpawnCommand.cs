using System;
using ConsoleAppEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using NUnit;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Text;

[Command("spawn", 2)]
[HelpCommandData("Spawns any object within the prefabe folder into the game", new string[] { "Prefab to spawn", "the position to spawn at written like \"0.1,04,01\"" })]
public class SpawnCommand : ICommand
{
    public void Execute(string[] args, KeyValuePair<string, string>[] options)
    {
        try
        {
            Addressables.LoadAssetAsync<GameObject>(args[0]).Completed += (AsyncOperationHandle<GameObject> obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = obj.Result;
                    string[] pos_data = args[1].Split(",");

                    Vector3 pos = new Vector3(float.Parse(pos_data[0]), float.Parse(pos_data[1]), float.Parse(pos_data[2]));

                    // Instantiate the prefab at the spawn point's position and rotation
                    MonoBehaviour.Instantiate(prefab, pos, Quaternion.identity).AddComponent<BoxCollider2D>().isTrigger = true;
                    Debug.Log("Object spawned");
                }
                else
                {
                    Debug.LogError("Prefab not found in Addressables");
                }
            };
        }
        catch (Exception e)
        {
            Debug.LogError("Execution of command failed due to an exception: " + e);
        }
    }

    public string additionalHelpData()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Addressables.InitializeAsync().Completed += (AsyncOperationHandle<IResourceLocator> obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                IResourceLocator resourceLocator = obj.Result; 
                IEnumerable<object> keys = resourceLocator.Keys;

                List<string> addresses = new List<string>();
                foreach (var key in keys)
                {
                    IList<IResourceLocation> locations;
                    if (resourceLocator.Locate(key, typeof(object), out locations))
                    {
                        foreach (var location in locations)
                        {
                            if(location.PrimaryKey.Contains("Prefabs") && location.PrimaryKey.Contains(".prefab"))
                                addresses.Add(location.PrimaryKey);
                        }
                    }
                }

                // Print all addresses
                foreach (string address in addresses)
                {
                    stringBuilder.AppendLine(address);
                    Debug.Log(address);
                }
            }
            else
            {
                Debug.LogError("Failed to initialize Addressables.");
            }
        };
        return stringBuilder.ToString();
    }
}