using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[Serializable]
public class DataHolder
{
    private Dictionary<string, object> settings = new Dictionary<string, object>();
    private bool Readonly = false;


    public object this[string index]
    {
        get { return GetData(index); }
        set { SetData(index, value); }
    }

    public DataHolder(bool _readonly = false)
    {
        Readonly = _readonly;
    }




    /// <summary>
    /// Loads the data from the given file
    /// </summary>
    /// <param name="path">The absolute file path</param>
    /// <param name="name">The File name and extension</param>
    /// <returns>true if succeeded in loading the false otherwise</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool Load(string path, string name = "Configs.bin")
    {
        string file = path + "/" + name;
        if (File.Exists(file))
        {
            FileStream fileS = File.OpenRead(file);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                settings =  (Dictionary<string, object>)formatter.Deserialize(fileS);
            }
            catch (Exception e)
            {
                fileS.Close();
                Debug.LogException(e);
                return false;
            }
            fileS.Close();
            return true;
        }  
        return false;
    }

    /// <summary>
    /// Saves the data to the given file
    /// </summary>
    /// <param name="path">The absolute file path</param>
    /// <param name="name">The File name and extension</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Save(string path, string name = "Configs.bin")
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileS = null;
        string file = path + "/" + name;
        try
        {
            if (!File.Exists(file))
            {
                fileS = File.Create(file);
            }
            else
            {
                fileS = File.OpenWrite(file);
            }
            formatter.Serialize(fileS, settings);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        fileS.Close();
    }

    /// <summary>
    /// Gets the data based on the given key
    /// </summary>
    /// <param name="key">The wanted data</param>
    /// <returns>The found value, null if nothing found</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public object GetData(string key)
    {
        return settings[key];
    }


    /// <summary>
    /// Sets the dat value based on the given key and data
    /// </summary>
    /// <param name="key">The wanted data</param>
    /// <param name="data">The value to set to</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void SetData(string key, object data)
    {
        if(Readonly)
            return;
        settings[key] = data;
    }

    /// <summary>
    /// Sets the data value based on the given key and data
    /// </summary>
    /// <param name="key">The wanted data</param>
    /// <param name="data">The value to set to</param>
    /// <typeparam name="t">The type to save the data as</typeparam>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void SetData<t>(string key, t data)
    {
        if (Readonly)
            return;
        settings[key] = (t)data;
    }


    /// <summary>
    /// Gets the data based on the given key
    /// </summary>
    /// <param name="key">The wanted data</param>
    /// <typeparam name="t">The type to load the data as</typeparam>
    /// <returns>The found value, null if nothing found</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public t GetData<t>(string key)
    {
        return (t)settings[key];
    }
}
