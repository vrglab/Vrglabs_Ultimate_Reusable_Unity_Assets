using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class InputManager : Singleton<InputManager>
{
    public InputProfile profile;

    public InputDevice ActiveInputDevice { get; private set; }

    public void OnEnable()
    {
        foreach (var item in profile.inputData)
        {
            item.Bindings.Enable();
        }
        InputUser.onChange += SetInputDevice;

        foreach (var device in InputSystem.devices)
        {
            Debug.Log("Found input device: " + device.path);
        }

        SetInputDevice();
    }

    public void OnDisable()
    {
        foreach (var item in profile.inputData)
        {
            item.Bindings.Disable();
        }
    }

    /// <summary>
    /// Check's if the asked action has a input binding if so it checks if any of those inputs were pressed on that fame
    /// </summary>
    /// <param name="actionID">The action id</param>
    /// <returns>True if the input was pressed down on</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool GetKeyDown(string actionID)
    {
        InputData in_d = GetInputData(actionID);
        return in_d.Bindings.WasPressedThisFrame();
    }

    /// <summary>
    /// Check's if the asked action has a input binding if so it checks if any of those inputs were released on that fame
    /// </summary>
    /// <param name="actionID">The action id</param>
    /// <returns>True if the input was released on</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public bool GetKeyUp(string actionID)
    {
        InputData in_d = GetInputData(actionID);
        return in_d.Bindings.WasReleasedThisFrame();
    }

    /// <summary>
    /// Get's the requested input data
    /// </summary>
    /// <param name="actionID">the action</param>
    /// <returns>The data returned by key</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public t GetValueData<t>(string actionID) where t : struct
    {
        InputData in_d = GetInputData(actionID);
        return in_d.Bindings.ReadValue<t>();
    }

    /// <summary>
    /// Get's us the all of requested actions key icons (Pc or Console)
    /// </summary>
    /// <param name="actionID">The action id</param>
    /// <returns>The key icons</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public List<Sprite> GetIcons(string actionID)
    {
        InputData in_d = GetInputData(actionID);
        List<Sprite> sprites = new List<Sprite>();
        foreach (var iconMapping in profile.RelativeIconMappings.KeyIcons)
        {
            foreach (var binding in in_d.Bindings.bindings)
            {
                if (iconMapping.PathContained(binding.path) && iconMapping.PathContained(ActiveInputDevice.path.Remove(0, 1)))
                {
                    sprites.Add(iconMapping.Image);
                }
            }
        }
        return sprites;
    }


    /// <summary>
    /// Get's us the requested actions key icon
    /// </summary>
    /// <param name="actionID">The action id</param>
    /// <param name="keyname">Key name</param>
    /// <returns>The key icon</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public Sprite GetIcon(string actionID, string keyname)
    {
        Sprite sprite = null;
        InputData in_d = GetInputData(actionID);

        foreach (var iconMapping in profile.RelativeIconMappings.KeyIcons)
        {
            foreach (var binding in in_d.Bindings.bindings)
            {
                if (binding.path.Contains(ActiveInputDevice.path.Remove(0, 1)) && binding.path.Contains(keyname))
                {
                    if (iconMapping.PathContained(binding.path))
                    {
                        sprite = iconMapping.Image;
                    }
                }
            }
        }
        return sprite;
    }

    /// <summary>
    /// Get's the requested actions bound <see cref="InputData"/> struct
    /// </summary>
    /// <param name="actionID">The requested action</param>
    /// <returns>The requested actions <see cref="InputData"/> struct</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public InputData GetInputData(string actionID)
    {
        foreach (var input_data in profile.inputData)
        {
            if (input_data.ActionID.Equals(actionID))
                return input_data;
        }

        return default;
    }

    /// <summary>
    /// Set's the input device to the requested device
    /// </summary>
    /// <remarks> Event function for the <see cref="InputUser"/></remarks>
    /// <param name="user">The user</param>
    /// <param name="change">The changed input user</param>
    /// <param name="device">The input device</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    private void SetInputDevice(InputUser user = default, InputUserChange change = default, InputDevice device = default)
    {
        if (Gamepad.current != null)
        {
            ActiveInputDevice = Gamepad.current;
            return;
        }

        if (Keyboard.current != null && Mouse.current != null)
        {
            ActiveInputDevice = Keyboard.current;
            return;
        }

        if (device != default)
        {
            ActiveInputDevice = device;
        }
    }
}
