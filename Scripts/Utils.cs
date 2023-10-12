using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Main Utility class
/// </summary>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class Utils
{
    /// <summary>
    /// Get's the component's in the given GameObject that have the given interface attached
    /// </summary>
    /// <typeparam name="T">The interface type</typeparam>
    /// <param name="resultList">List off all component's with the interface</param>
    /// <param name="objectToSearch">The GameObject containing the component's</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T : class
    {
        MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
        resultList = new List<T>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is T)
            {
                //found one
                resultList.Add((T)((System.Object)mb));
            }
        }
    }

    /// <summary>
    /// Check's if the given GameObject has component attached that has the given interface type on it
    /// </summary>
    /// <typeparam name="T">The given interface type</typeparam>
    /// <param name="objectToSearch">The given GameObject</param>
    /// <returns>True if the interface is present on a component</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static bool HasInterfaces<T>(GameObject objectToSearch) where T : class
    {
        MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour mb in list)
        {
            if (mb is T)
            {
                //found one
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Updates the shape of the polygon collider to be the same as the sprite
    /// </summary>
    /// <param name="polygonCollider2D">The collider</param>
    /// <param name="sprite">The sprite</param>
    /// <param name="tolerance">How accurate it is</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static void UpdatePolygonCollider2D(PolygonCollider2D polygonCollider2D, Sprite sprite, float tolerance = 0.05f)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> simplifiedPoints = new List<Vector2>();
        polygonCollider2D.pathCount = sprite.GetPhysicsShapeCount();
        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            sprite.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, tolerance, simplifiedPoints);
            polygonCollider2D.SetPath(i, simplifiedPoints);
        }
    }

    /// <summary>
    /// Executes a Action after the timer run's out
    /// </summary>
    /// <param name="timeInSeconds">The time to wait for</param>
    /// <param name="action">What to execute</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    /// <br>Leon Hefling (700Noel)</br>
    public static IEnumerator TimedExecution(float timeInSeconds, Action action)
    {
        yield return new WaitForSeconds(timeInSeconds);
        action.Invoke();
        yield return true;
    }

    /// <summary>
    /// Starts a Coroutine
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    /// <br>Leon Hefling (700Noel)</br>
    public static void StartCoroutine(IEnumerator Coroutine, float waitTime = 0.01f)
    {
        GameObject gameObject = new GameObject("Coroutine");
        gameObject.AddComponent<CoroutineStarter>().StartCoroutine(Coroutine);
        MonoBehaviour.Destroy(gameObject, waitTime);

    }

    private class CoroutineStarter : MonoBehaviour
    {
        public void startCoroutine(IEnumerator Coroutine)
        {
            StartCoroutine(Coroutine);
        }
    }

    /// <summary>
    /// Loops an action for a specified amount in a specified interval
    /// </summary>
    /// <param name="amount">The amount of intervals</param>
    /// <param name="waitPerAmount">The length of each interval</param>
    /// <param name="action">The executed action in each loop</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    /// <br>Leon Hefling (700Noel)</br>
    public static IEnumerator NumberedExecution(int amount, int waitPerAmount, Action action, Action OnDone = default)
    {
        for (int i = 0; i < amount; i++)
        {
            action.Invoke();
            if (i + 1 == amount)
            {
                if (OnDone != null)
                    OnDone.Invoke();
            }
            yield return new WaitForSeconds(waitPerAmount);
        }
        yield return null;
    }

    /// <summary>
    /// Loops an action for a specified amount in a specified interval
    /// </summary>
    /// <param name="amount">The amount of intervals</param>
    /// <param name="waitPerAmount">The length of each interval</param>
    /// <param name="action">The executed action in each loop</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    /// <br>Leon Hefling (700Noel)</br>
    public static IEnumerator NumberedExecution(int amount, float waitPerAmount, Action action, Action OnDone = default)
    {
        for (int i = 0; i < amount; i++)
        {
            action.Invoke();
            if (i + 1 == amount && OnDone != null)
            {
                OnDone.Invoke();
            }
            yield return new WaitForSeconds(waitPerAmount);
        }
    }

    public static Vector2 LooAt(Vector2 us, Vector2 subject)
    {
        return (subject - new Vector2(us.x, us.y));
    }
}

/// <summary>
/// Gives a instance to the class that inherits it
/// </summary>
/// <typeparam name="t">The class to make the instance for</typeparam>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public abstract class Instancable<t> : MonoBehaviour where t : MonoBehaviour
{
    public static t Instance { get; protected set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as t;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

/// <summary>
/// Gives a instance to the class that inherits it and makes sure only one of this class exists
/// </summary>
/// <typeparam name="t">The class to make the instance for</typeparam>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public abstract class Singleton<t> : Instancable<t> where t : MonoBehaviour
{

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this as t;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

/// <summary>
/// Gives a instance to the class that inherits it, makes sure only one of this class exists and that it is present in each scene within the game
/// </summary>
/// <typeparam name="t">The class to make the instance for</typeparam>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public abstract class PersistantSingleton<t> : Singleton<t> where t : MonoBehaviour
{

    protected override void Awake()
    {
        setInstance();
    }

    private void setInstance()
    {
        if (Instance == null)
        {
            Instance = this as t;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
