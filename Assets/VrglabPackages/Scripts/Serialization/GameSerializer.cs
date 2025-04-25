using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameSerializer : Instancable<GameSerializer>
{
    [SerializeField]
    private List<SerializationSlotData> slots = new List<SerializationSlotData>(); 


    public SerializationSlotData CurrentlyActiveSlot {  get; private set; }


    public void Awake()
    {
        base.Awake();
        foreach (var slot in Directory.GetFiles(Application.persistentDataPath))
        {
            FileInfo fileInfo = new FileInfo(slot);
            if (fileInfo.Exists && fileInfo.Extension == ".sav")
            {
                slots.Add(Load(slot));
            }
        }
    }

    public bool HasActiveSlot()
    {
        return (CurrentlyActiveSlot.SlotId != Guid.Empty && CurrentlyActiveSlot.saved_data.Count > 0);
    }

    public void SaveSlot(Guid slot_id)
    {
        string filePath = Application.persistentDataPath + "/" + slot_id + ".sav";

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = null;
        SerializationSlotData found_slot = default;

        if (File.Exists(filePath))
        {
            stream = File.OpenWrite(filePath);
        } else
        {
            stream = File.Create(filePath);
        }

        foreach (var slot in slots)
        {
            if (slot.SlotId == slot_id)
            {
                found_slot = slot;
                break;
            }
        }

        found_slot.lastSavedOn = DateTime.UtcNow.ToString();

        try
        {
            binaryFormatter.Serialize(stream, found_slot);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            stream.Close();
        }

        stream.Close();
    }

    public void SelectSlot(Guid slot_id)
    {
        foreach (var slot in slots)
        {
            if(slot.SlotId == slot_id)
            {
                CurrentlyActiveSlot = slot;
                return;
            }
        }
    }

    public SerializationSlotData CreateSlot()
    {
        Guid slot_id = Guid.NewGuid();
        SerializationSlotData slot = new SerializationSlotData();
        slot.SlotId = slot_id;
        slot.saved_data = new Dictionary<string, DataHolder>();
        slot.loaded_levels = LevelManager.Instance.levels;
        slot.lastSavedOn = DateTime.UtcNow.ToString();
        slots.Add(slot);
        return slot;
    }

    public static SerializationSlotData Load(string path)
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream stream = File.OpenRead(path);

        SerializationSlotData data = (SerializationSlotData)binaryFormatter.Deserialize(stream);
        stream.Close();
        return data;
    }
}

[Serializable]
public struct SerializationSlotData
{
    public Guid SlotId;
    public string lastSavedOn;
    public Dictionary<string, DataHolder> saved_data;
    public Dictionary<string, LevelInstance> loaded_levels;
}
