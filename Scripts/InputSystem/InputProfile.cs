using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
[CreateAssetMenu(menuName = "Input System/Input Profile")]
public class InputProfile : ScriptableObject
{
    public InputIconMappingProfile RelativeIconMappings;
    public List<InputData> inputData;
}

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
[Serializable]
public struct InputData
{
    public string ActionID;

    public InputAction Bindings;
}