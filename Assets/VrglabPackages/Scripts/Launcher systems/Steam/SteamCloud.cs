using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UWP && !UNITY_ANDROID && !UNITY_IPHONE
using Steamworks;
#endif
using System.IO;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class SteamCloud : Instancable<SteamCloud>
{
#if !UWP && !UNITY_ANDROID  && !UNITY_IPHONE
    /// <summary>
    /// Read's the byte inside the file and writes it into the cloud
    /// </summary>
    /// <param name="name">File name to use as id</param>
    /// <param name="filePath">The path to the file</param>
    /// <returns>True if saved in the cloud</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool WriteFileToCloud(string name, string filePath)
    {
        string c_Name = name + ".binsav";
       return SteamRemoteStorage.FileWrite(c_Name, File.ReadAllBytes(filePath));
    }

    /// <summary>
    /// Writes the provided bytes into the cloud
    /// </summary>
    /// <param name="name">File name to use as id</param>
    /// <param name="file">File bytes</param>
    /// <returns>True if saved in the cloud</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool WriteFileToCloud(string name, byte[] file)
    {
        string c_Name = name + ".binsav";
        return SteamRemoteStorage.FileWrite(c_Name, file);
    }

    /// <summary>
    /// Read's the file data saved in the cloud
    /// </summary>
    /// <param name="name">The file name without a extension</param>
    /// <returns>The read byte's</returns>
    /// <exception cref="System.IO.FileNotFoundException"></exception>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public byte[] GetFileData(string name)
    {
        string c_Name = name + ".binsav";
        if (!FileExists(name, "binsav"))
        {
            throw new System.IO.FileNotFoundException();
        }
        return SteamRemoteStorage.FileRead(c_Name);
    }

    /// <summary>
    /// Check's if the file exists in the cloud
    /// </summary>
    /// <param name="name">The file name</param>
    /// <param name="extension">The file extension</param>
    /// <returns>True if the file exists</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool FileExists(string name, string extension)
    {
        return SteamRemoteStorage.FileExists(name + "." + extension);
    }
#endif
}