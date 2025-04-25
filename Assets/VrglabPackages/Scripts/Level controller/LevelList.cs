using Ink.Parsed;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "LevelList", menuName = "Level Manager/Level List")]
public class LevelList : ScriptableObject
{
    public List<Region> regions;

    public Region GetRegionByName(string name)
    {
        foreach (var region in regions)
        {
            if(region.Name == name)
            {
                return region;
            }
        }
        throw new NotFoundError();
    }


    public Level GetLevelFromRegion(string regionName, string levelName)
    {
        Region region = GetRegionByName(regionName);

        foreach (var level in region.Levels)
        {
            if(level.Name == levelName)
            {
                return level;
            }
        }
        throw new NotFoundError();
    }
}

[Serializable]
public struct Region
{
    public string Name;
    public List<Level> Levels;
}


[Serializable]
public struct Level
{
    public string Name;
#if UNITY_ANDROID || UNITY_IPHONE
    public bool PhoneUIEnabled;
#endif
    public List<LevelData> data;
    public List<Neighbour> neighbours;
}

[Serializable]
public struct LevelData
{
    public string key;
    public string defaultValue;
    public Types parseAs;
}

[Serializable]
public struct Neighbour
{
    public Directions atOur;
    public string Name;

    [Tooltip("Leave empty if Neighbouring level is in the same region")]
    public string Region;

    public Trigger.TriggerDirection ToTriggerDir()
    {
        switch (atOur)
        {
            case Directions.Up:
                return Trigger.TriggerDirection.FromBelow;
            case Directions.Down:
                return Trigger.TriggerDirection.FromAbove;
            case Directions.Left:
                return Trigger.TriggerDirection.FromRight;
            case Directions.Right:
                return Trigger.TriggerDirection.FromLeft;
            default:
                return Trigger.TriggerDirection.Any;
        }
    }
}