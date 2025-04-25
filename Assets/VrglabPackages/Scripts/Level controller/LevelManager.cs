using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LevelManager : Instancable<LevelManager>
{
    [SerializeField]
    private LevelList levelList;


    public Dictionary<string, LevelInstance> levels { get; set; } = new Dictionary<string, LevelInstance>();


    public LevelInstance CurrentLevel { get; private set; }

    public DataHolder data = new DataHolder();


    public void LoadLevel(string levelName, string levelRegion, DataHolder loadedData = null)
    {
        Level level = levelList.GetLevelFromRegion(levelRegion, levelName);
        if(LevelInstanceHandler.Instance != null)
            LevelInstanceHandler.Instance.OnLevelUnloaded();
        try
        {
            if(!String.IsNullOrEmpty(CurrentLevel.levelName))
            {
                levels[CurrentLevel.levelName] = CurrentLevel;
            }

            LevelInstance loaded_instance = levels[levelName];
            CurrentLevel = loaded_instance;
            data["current_level"] = levelName;
            data["current_region"] = levelRegion;
        }
        catch (Exception e)
        {
            if (CurrentLevel.levelName != String.Empty && levels.Count > 0)
            {
                levels[CurrentLevel.levelName] = CurrentLevel;
            }

            DataHolder levelData = null;

            if (loadedData == null)
            {
                levelData = new DataHolder();
                foreach (var data in level.data)
                {
                    switch (data.parseAs)
                    {
                        case Types.Boolean:
                            levelData.SetData(data.key, Boolean.Parse(data.defaultValue));
                            break;

                    }
                }
            } else
            {
                levelData = loadedData;
            }

            LevelInstance levelInstance = new LevelInstance
            {
                levelName = level.Name,
                levelRegion = levelRegion,
                Level = level,
                LevelData = levelData,
                ActiveItemsInLevel = new Dictionary<string, DataHolder>()
            };

            CurrentLevel = levelInstance;
            levels.Add(level.Name, levelInstance);
            data["current_level"] = levelName;
            data["current_region"] = levelRegion;
        }
        GameSerializer.Instance.CurrentlyActiveSlot.saved_data["level_info"] = data;
        LoadingController.Instance.LoadTheScene(level.Name);
    }

    public void GoToNeighbour(Directions atOur)
    {
        foreach (var neighbour in CurrentLevel.Level.neighbours)
        {
            if(neighbour.atOur == atOur)
            {
                LoadLevel(neighbour.Name, (neighbour.Region == String.Empty ? CurrentLevel.levelRegion : neighbour.Region));
                return;
            }
        }
        throw new NotFoundError($"No Neighbour found towards our: {atOur}");
    }
}

[Serializable]
public struct LevelInstance
{
    public string levelName;
    public string levelRegion;
    public Level Level;
    public DataHolder LevelData;
    public Dictionary<string, DataHolder> ActiveItemsInLevel;
}