using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input System/Icon Mapping profile")]
public class InputIconMappingProfile : ScriptableObject
{
    public KeyIconMapping[] KeyIcons;
}

[System.Serializable]
public struct KeyIconMapping
{
    public Sprite Image;
    public string Name;
    public List<string> paths;

    public bool PathContained(string path)
    {
        foreach (var pathItem in paths)
        {
            if (pathItem.Contains(path))
            {
                return true;
            }
        }
        return false;
    }

    public bool PathEquals(string path)
    {
        foreach (var pathItem in paths)
        {
            if (pathItem.Equate(path, 1))
            {
                return true;
            }
        }
        return false;
    }
}