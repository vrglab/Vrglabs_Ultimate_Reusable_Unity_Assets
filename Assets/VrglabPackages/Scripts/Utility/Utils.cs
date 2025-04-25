using FMOD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Main Utility class
/// </summary>
/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public static class Utils
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
    /// Executes a Action after the timer runData's out
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
    public static void StartCoroutine(IEnumerator Coroutine, float waitTime = 0.01f, GameObject gameObject = null)
    {
        if(gameObject == null)
            gameObject = new GameObject("Coroutine");

        gameObject.AddComponent<CoroutineStarter>().StartCoroutine(Coroutine);
        if(gameObject.name == "Coroutine")
            MonoBehaviour.Destroy(gameObject, waitTime);
        else
            MonoBehaviour.Destroy(gameObject.GetComponent<CoroutineStarter>(), waitTime);
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
        yield return NumberedExecution(amount, (float)waitPerAmount, action, OnDone);
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
                if (OnDone != null)
                    OnDone.Invoke();
            }
            yield return new WaitForSeconds(waitPerAmount);
        }
    }

    public static Vector2 LooAt(Vector2 us, Vector2 subject)
    {
        return (subject - new Vector2(us.x, us.y));
    }

    public static object ConvertToBestType(string input)
    {
        // Try to convert to int
        if (int.TryParse(input, out int intResult))
        {
            return intResult;
        }

        // Try to convert to float
        if (float.TryParse(input, out float floatResult))
        {
            return floatResult;
        }

        // Try to convert to bool
        if (bool.TryParse(input, out bool boolResult))
        {
            return boolResult;
        }

        // Try to convert to binary
        if (IsBinary(input))
        {
            return ConvertBinaryToInt(input);
        }

        // If all conversions fail, return the original string
        return input;
    }

    private static bool IsBinary(string input)
    {
        // Check if the string is a valid binary number (consists of only 0s and 1s)
        return Regex.IsMatch(input, "^[01]+$");
    }

    private static int ConvertBinaryToInt(string binary)
    {
        // Convert binary string to integer
        return Convert.ToInt32(binary, 2);
    }
}


public static class Extensions
{

    public static bool IsChildOf(this Type child, Type parent)
    {
        // Check if the childType is a direct child of the firstLayerParentType
        if (child.BaseType == parent)
        {
            return true;
        }

        // Check if childType has a base type and recursively check its base type
        if (child.BaseType != null)
        {
            return IsChildOf(child.BaseType, parent);
        }

        return false;
    }

    public static string RemoveSpecialCharacters(this string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if (c != '\n' && c != '\r' && c != '\t' && c != '\a' && c != '\f')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static bool Equate(this string str, string st2, int start_pos)
    {
        bool result = true;
        if (str.Length != st2.Length)
            return false;
        for (int i = start_pos; i < str.Length; i++)
        {
            if (str[i] != st2[i])
            {
                result = false;
            }
        }
        return result;
    }

    public static bool contains(this IEnumerable<JProperty> source, string id)
    {
        foreach (JProperty t in source)
        {
            if (t.Name == id)
            {
                return true;
            }
        }
        return false;
    }

    public static string RemoveTabbing(this string fmt)
    {
        return string.Join(
            System.Environment.NewLine,
            fmt.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(fooline => fooline.Trim()));
    }

    public static string GetOptionValue(this KeyValuePair<string, string>[] options, string valueToSearch)
    {
        foreach (var option in options)
        {
            if (option.Key == valueToSearch)
            {
                return option.Value;
            }
        }
        return String.Empty;
    }
    public static bool Contains(this KeyValuePair<string, string>[] options, string valueToSearch)
    {
        foreach (var option in options)
        {
            if (option.Key == valueToSearch)
            {
                return true;
            }
        }
        return false;
    }

    public static Vector2 ParseDirection(this Directions dir)
    {
        switch (dir)
        {
            case Directions.Up:
                return Vector2.down;
            case Directions.Down:
                return Vector2.up;
            case Directions.Left:
                return Vector2.left;
            case Directions.Right:
                return Vector2.right;
            default: return Vector2.left;
        }
    }

    public static string ToIsoCode(this SystemLanguage language)
    {
        return language switch
        {
            SystemLanguage.English => "en",
            SystemLanguage.German => "de",
            SystemLanguage.French => "fr",
            SystemLanguage.Spanish => "es",
            SystemLanguage.Japanese => "ja",
            SystemLanguage.Chinese => "zh",
            SystemLanguage.ChineseSimplified => "zh-Hans",
            SystemLanguage.ChineseTraditional => "zh-Hant",
            SystemLanguage.Korean => "ko",
            SystemLanguage.Russian => "ru",
            SystemLanguage.Italian => "it",
            SystemLanguage.Portuguese => "pt",
            SystemLanguage.Dutch => "nl",
            SystemLanguage.Swedish => "sv",
            SystemLanguage.Norwegian => "no",
            SystemLanguage.Finnish => "fi",
            SystemLanguage.Danish => "da",
            SystemLanguage.Polish => "pl",
            SystemLanguage.Turkish => "tr",
            SystemLanguage.Arabic => "ar",
            SystemLanguage.Hebrew => "he",
            _ => "en" // fallback to English if unknown
        };
    }
}

public enum Directions
{
    Up, Down, Left, Right
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
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
