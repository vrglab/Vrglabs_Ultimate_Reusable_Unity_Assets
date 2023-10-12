using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class StringProccessor : Instancable<StringProccessor>
{
    /// <summary>
    /// Basically same as <see cref="LocalizationManager.GetEntry(string)"/> We just replace the tokens inside of the text with whatever is needed
    /// </summary>
    /// <typeparam name="t">The class containing Token definitions like a .cpp in c++</typeparam>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string GetEntry<t>(string textId, object caller = default) where t : DefaultTokenFunctionalityHandlers
    {
        string text = LocalizationManager.Instance.GetEntry(textId);
        List<Token> tokens = TokenParser.Instance.Parse(text);

        List<string> data = new List<string>();

        Type type = typeof(t);

        foreach (var token in tokens)
        {
            bool methodFound = false;
            foreach (var method in type.GetMethods())
            {
                if (method.Name.Equals(token.ID) && (method.GetParameters().Length - 1) == token.Arguments.Count)
                {
                    List<object> arguments = new List<object>();

                    foreach (var argument in token.Arguments)
                    {
                        switch (argument.Type)
                        {
                            case Types.Object:
                                arguments.Add(argument.value);
                                break;
                            case Types.String:
                                arguments.Add(argument.value.ToString());
                                break;
                            case Types.Boolean:
                                arguments.Add(Boolean.Parse(argument.value));
                                break;
                            case Types.Int:
                                arguments.Add(int.Parse(argument.value));
                                break;
                            case Types.Float:
                                arguments.Add(float.Parse(argument.value));
                                break;
                            case Types.Byte:
                                arguments.Add(byte.Parse(argument.value));
                                break;
                        }
                    }

                    arguments.Add(caller);

                    var get = method.Invoke(null, arguments.ToArray());

                    data.Add(get.ToString());
                    methodFound = true;
                }
            }
            if (!methodFound)
            {
                data.Add("");
            }
        }

        return TokenParser.Instance.ReplaceTokens(text, data);
    }

    /// <summary>
    /// Basically same as <see cref="LocalizationManager.GetEntry(string)"/> We just replace the tokens inside of the text with whatever is needed
    /// </summary>
    /// <typeparam name="t">The class containing Token definitions like a .cpp in c++</typeparam>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string GetEntry(string textId, object caller = default)
    {
        return GetEntry<DefaultTokenFunctionalityHandlers>(textId, caller);
    }
}

/// <b>Authors</b>
/// <br>Arad Bozorgmehr (Vrglab)</br>
public class DefaultTokenFunctionalityHandlers
{
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static string InputAct(string action, string keyStatus, object caller)
    {
        var in_d = InputManager.Instance.GetInputData(action);

        return "null";
    }

    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static string Img(string spriteAsset, float posX, float posY, float Width, float Height, int spaceBetweenText, string ObjectToAttachTo, string ParentComponent, string EventName, object caller)
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(spriteAsset);
        // Register a callback to handle the completion of the sprite loading
        handle.Completed += spriteOperation =>
        {
            if (spriteOperation.Status == AsyncOperationStatus.Succeeded)
            {
                Sprite newImage = spriteOperation.Result;

                GameObject image = new GameObject(spriteAsset, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                image.GetComponent<RectTransform>().SetParent(GameObject.FindWithTag("MainCanvas").transform);
                image.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                image.GetComponent<RectTransform>().localScale = new Vector3(Width, Height, 0);
                image.GetComponent<Image>().sprite = newImage;

                try
                {
                    if (caller is GameObject)
                    {
                        var obj = caller as GameObject;
                        image.GetComponent<RectTransform>().SetParent(obj.transform);

                        if (obj.name.Equals(ObjectToAttachTo))
                        {
                            Type type = Type.GetType(ParentComponent);

                            if (obj.GetComponent(type) != null)
                            {
                                foreach (var item in type.GetFields())
                                {
                                    if (item.Name.Equals(EventName))
                                    {
                                        if (item.FieldType == typeof(UnityEvent))
                                        {
                                            var eventParsed = item.GetValue(obj.GetComponent(type)) as UnityEvent;

                                            eventParsed.AddListener(() =>
                                            {
                                                GameObject.Destroy(image);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    image.GetComponent<RectTransform>().SetParent(GameObject.FindWithTag("MainCanvas").transform);
                }
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + spriteAsset);
            }
        };

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < spaceBetweenText; i++)
        {
            sb.Append(" ");
        }
        return sb.ToString();
    }

    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public static string InputImg(string action, int keyimgindex, float posX, float posY, float Width, float Height, int spaceBetweenText, string ObjectToAttachTo, string ParentComponent, string EventName, object caller)
    {
        Sprite sp = InputManager.Instance.GetIcons(action)[keyimgindex];

        if (sp == null)
        {
            sp = Sprite.Create(Texture2D.normalTexture, Rect.MinMaxRect(0, 0, 1, 1), Vector2.zero);
            sp.name = action;
        }


        GameObject image = new GameObject(sp.name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        image.GetComponent<RectTransform>().SetParent(GameObject.FindWithTag("MainCanvas").transform);
        image.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
        image.GetComponent<RectTransform>().localScale = new Vector3(Width, Height, 0);
        image.GetComponent<Image>().sprite = sp;

        try
        {
            if (caller is GameObject)
            {
                var obj = caller as GameObject;
                image.GetComponent<RectTransform>().SetParent(obj.transform);

                if (obj.name.Equals(ObjectToAttachTo))
                {
                    Type type = Type.GetType(ParentComponent);

                    if (obj.GetComponent(type) != null)
                    {
                        foreach (var item in type.GetFields())
                        {
                            if (item.Name.Equals(EventName))
                            {
                                if (item.FieldType == typeof(UnityEvent))
                                {
                                    var eventParsed = item.GetValue(obj.GetComponent(type)) as UnityEvent;

                                    eventParsed.AddListener(() =>
                                    {
                                        GameObject.Destroy(image);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            image.GetComponent<RectTransform>().SetParent(GameObject.FindWithTag("MainCanvas").transform);
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < spaceBetweenText; i++)
        {
            sb.Append(" ");
        }
        return sb.ToString();
    }
}