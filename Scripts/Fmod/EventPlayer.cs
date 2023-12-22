using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EventPlayer : MonoBehaviour
{
    public EventReference refrance;

    private EventInstance instance;

    public void Update()
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    /// <summary>
    /// Plays a instance of the Reference
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Play()
    {
        instance = RuntimeManager.CreateInstance(refrance);
        RuntimeManager.AttachInstanceToGameObject(instance, GetComponent<Transform>(), GetComponent<Rigidbody2D>());
        instance.start();
    }

    /// <summary>
    /// Stops the current instance that is playing with a fade out
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Stop()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }

    /// <summary>
    /// Stops the current that is playing instantly
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void InstantStop()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
}
