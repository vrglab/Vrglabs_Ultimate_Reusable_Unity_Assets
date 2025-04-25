using Ink.Parsed;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelObjectSpawner : Instancable<LevelObjectSpawner>
{
    [Serializable]
    public class ObjectData
    {
        public string Name;
        public WorldObject item;
        public GameObject Prefab;
        public Vector3 Position;
    }

    [SerializeField]
    private List<ObjectData> _objects = new List<ObjectData>();

    public void OnLevel(DataHolder LevelData, Dictionary<string, DataHolder> instance_items)
    {
        foreach (var obj in instance_items)
        {
            WorldObject item = IUWRegistery.Instance.ResolveIUW(obj.Key);

            if(!isItemOriginHere(item))
            {
                GameObject ConstructedPrefab = new GameObject(item.name + "_prefab");
                ConstructedPrefab.AddComponent<SpriteRenderer>();
                ConstructedPrefab.AddComponent<WorldItem>().ItemType = item;
                ConstructedPrefab.AddComponent<BoxCollider2D>().size = new Vector2(1, 1);
                ConstructedPrefab.GetComponent<BoxCollider2D>().isTrigger = true;

                ConstructedPrefab.transform.position = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
                try
                {
                    DataHolder iuw_data_core = LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name));
                    if (!iuw_data_core.GetData<bool>(DataIds.getIUWPickupStateKey(item.name)))
                    {
                        try
                        {
                            Spawn(item, iuw_data_core.GetData<DataHolder>(DataIds.getIUWPositionKey(item.name)).ParseToVector3(), obj: ConstructedPrefab);
                        }
                        catch (Exception)
                        {
                            Spawn(item, obj.Value.GetData<DataHolder>(DataIds.getIUWPositionKey(item.name)).ParseToVector3());
                        }
                    }
                }
                catch (Exception e)
                {

                    try
                    {
                        DataHolder iuw_data_core = obj.Value.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name));
                        if (!iuw_data_core.GetData<bool>(DataIds.getIUWPickupStateKey(item.name)))
                        {
                            try
                            {
                                Spawn(item, iuw_data_core.GetData<DataHolder>(DataIds.getIUWPositionKey(item.name)).ParseToVector3(), obj: ConstructedPrefab);
                            }
                            catch (Exception)
                            {
                                Spawn(item, obj.Value.GetData<DataHolder>(DataIds.getIUWPositionKey(item.name)).ParseToVector3());
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ObjectData d = new ObjectData()
                        {
                            Name = item.name,
                            item = item,
                            Position = obj.Value.GetData<DataHolder>(DataIds.getIUWPositionKey(item.name)).ParseToVector3(),
                            Prefab = ConstructedPrefab
                        };

                        Spawn(d);
                    }
                }
            }
        }

        foreach (var obj in _objects)
        {
            if(obj.item == null)
            {
                spawn_obj_onLevelLoad(LevelData, obj, obj.Name, (obj, iuw_data_core) => {
                    obj.Position = iuw_data_core.GetData<DataHolder>(DataIds.getIUWPositionKey(obj.Name)).ParseToVector3();
                    Spawn(obj);
                });
            }
            else
            {
                spawn_obj_onLevelLoad(LevelData, obj, obj.item.name, (obj, iuw_data_core) => {
                    Spawn(obj.item, iuw_data_core.GetData<DataHolder>(DataIds.getIUWPositionKey(obj.item.name)).ParseToVector3(), obj: obj.Prefab);
                });
            }
        }
    }

    private void spawn_obj_onLevelLoad(DataHolder LevelData, ObjectData obj,  string name, Action<ObjectData, DataHolder> base_spawn)
    {
        try
        {
            DataHolder iuw_data_core = LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(name));
            if (!iuw_data_core.GetData<bool>(DataIds.getIUWPickupStateKey(name)))
            {
                try
                {
                    base_spawn(obj, iuw_data_core);
                }
                catch (Exception)
                {
                    Spawn(obj);
                }
            }
        }
        catch (Exception e)
        {
            Spawn(obj);
        }
    }

    public bool isItemOriginHere(WorldObject item)
    {
        foreach (var obj in _objects)
        {
            if(obj.item == item)
            {
                return true;
            }
        }

        return false;
    }


    public void Spawn(WorldObject item, Vector3 Position, Action onSpawn = null, Action<GameObject> additionalGameObjectData = null, GameObject obj = null)
    {
        if (obj == null)
        {
            obj = new GameObject(item.name);
            obj.transform.position = Position;
        }
        else
        {
            obj = Instantiate(obj, Position, Quaternion.identity);
        }
        if(obj.GetComponent<WorldItem>() == null)
        {
            WorldItem worldItem = obj.AddComponent<WorldItem>();
            worldItem.ItemType = item;
        }  

        additionalGameObjectData?.Invoke(obj);
        onSpawn?.Invoke();

        try
        {
            if (LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name)).GetData<bool>(DataIds.getIUWPickupStateKey(item.name)))
            {
                LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name)).SetData(DataIds.getIUWPickupStateKey(item.name), false);
            }
        }
        catch (Exception)
        {
            try
            {
                LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name)).SetData(DataIds.getIUWPickupStateKey(item.name), false);
            }
            catch (Exception)
            {
                LevelManager.Instance.CurrentLevel.LevelData.SetData(DataIds.getIUWCoreDataKey(item.name), new DataHolder());
                LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name)).SetData(DataIds.getIUWPickupStateKey(item.name), false);
            }
        }

        LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(item.name)).SetData(DataIds.getIUWPositionKey(item.name), new DataHolder().SerializeVector3(Position));
        
        if(!LevelManager.Instance.CurrentLevel.ActiveItemsInLevel.ContainsKey(IUWRegistery.Instance.ResolveId(item)))
        {
            DataHolder dataHolder = new DataHolder();
            dataHolder.SetData(DataIds.iuw_name, item.name);
            dataHolder.SetData(DataIds.getIUWPositionKey(item.name), new DataHolder().SerializeVector3(Position));
            dataHolder.SetData(DataIds.getIUWPickupStateKey(item.name), false);
            LevelManager.Instance.CurrentLevel.ActiveItemsInLevel.Add(IUWRegistery.Instance.ResolveId(item), dataHolder);
        } else
        {
            LevelManager.Instance.CurrentLevel.ActiveItemsInLevel[IUWRegistery.Instance.ResolveId(item)].SetData(DataIds.getIUWPickupStateKey(item.name), true);
        }
    }

    public void Spawn(ObjectData obj, Action onSpawn = null, Action<GameObject> additionalGameObjectData = null)
    {
        GameObject oj = Instantiate(obj.Prefab, obj.Position, Quaternion.identity);
        additionalGameObjectData?.Invoke(oj);
        onSpawn?.Invoke();

        string id = "";

        if(obj.item != null)
        {
            id = obj.item.name;

            if (!LevelManager.Instance.CurrentLevel.ActiveItemsInLevel.ContainsKey(IUWRegistery.Instance.ResolveId(obj.item)))
            {
                DataHolder dataHolder = new DataHolder();
                dataHolder.SetData(DataIds.iuw_name, id);
                dataHolder.SetData(DataIds.getIUWPositionKey(id), new DataHolder().SerializeVector3(obj.Position));
                dataHolder.SetData(DataIds.getIUWPickupStateKey(id), false);
                LevelManager.Instance.CurrentLevel.ActiveItemsInLevel.Add(IUWRegistery.Instance.ResolveId(obj.item), dataHolder);
            }
            else
            {
                LevelManager.Instance.CurrentLevel.ActiveItemsInLevel[IUWRegistery.Instance.ResolveId(obj.item)].SetData(DataIds.getIUWPickupStateKey(id), true);
            }
        }
        else
        {
            id = obj.Name;
        }

        LevelManager.Instance.CurrentLevel.LevelData.SetData(DataIds.getIUWCoreDataKey(id), new DataHolder());
        LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(id)).SetData(DataIds.getIUWPickupStateKey(id), false);
        LevelManager.Instance.CurrentLevel.LevelData.GetData<DataHolder>(DataIds.getIUWCoreDataKey(id)).SetData(DataIds.getIUWPositionKey(id), new DataHolder().SerializeVector3(obj.Position));
    }
}
