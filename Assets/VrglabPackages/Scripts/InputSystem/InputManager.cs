using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Playables;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class InputManager : PersistantSingleton<InputManager>
{
    public InputProfile profile;

    public InputDevice ActiveInputDevice { get; private set; }

    private Dictionary<string, object> manualOverrides = new();

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

    public void SetManualOverride<T>(string actionID, T value) where T : struct
    {
        manualOverrides[actionID] = value;
    }

    public void ClearManualOverride(string actionID)
    {
        if (manualOverrides.ContainsKey(actionID))
            manualOverrides.Remove(actionID);
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
        if (manualOverrides.TryGetValue(actionID, out var manualValue) && manualValue is bool)
            return Boolean.Parse(manualValue.ToString());


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
        if (manualOverrides.TryGetValue(actionID, out var manualValue) && manualValue is bool)
            return Boolean.Parse(manualValue.ToString());

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
        if (manualOverrides.TryGetValue(actionID, out var manualValue) && manualValue is t tVal)
            return tVal;

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
                if(Keyboard.current != null && Gamepad.current == null)
                {
                    if (iconMapping.PathEquals(binding.path))
                    {
                        sprites.Add(iconMapping.Image);
                    }
                } else
                {
                    if (iconMapping.PathEquals(binding.path) && iconMapping.PathContained(ActiveInputDevice.path.Remove(0, 1)))
                    {
                        sprites.Add(iconMapping.Image);
                    }
                }
                
            }
        }
        return sprites;
    }

    /// <summary>
    /// Get's us the <seealso cref="KeyIconMapping"/> for the key requested
    /// </summary>
    /// <param name="path">The key path</param>
    /// <returns>The found KeyIconMapping</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public KeyIconMapping GetKeyIconMapping(string path)
    {
        foreach (var iconMapping in profile.RelativeIconMappings.KeyIcons)
        {
            if (iconMapping.PathEquals(path) && iconMapping.PathContained(ActiveInputDevice.path.Remove(0, 1)))
            {
              return iconMapping;
            }
        }
        return default;
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

        if (Touchscreen.current != null)
        {
            ActiveInputDevice = Touchscreen.current;
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

    public bool GetKeyHeld(string actionID)
    {
        InputData in_d = GetInputData(actionID);
        return in_d.Bindings.IsPressed();
    }
}
