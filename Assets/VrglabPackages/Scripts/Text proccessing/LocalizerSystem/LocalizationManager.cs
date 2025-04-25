using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using static LocalizationManager;
using static UnityEngine.EventSystems.EventTrigger;

public class LocalizationManager : PersistantSingleton<LocalizationManager>
{
    public string LangID;
    public string ActiveApplicationLang { get; private set; } = "English";

    private Dictionary<string, List<LangData>> loadedLanguages = new Dictionary<string, List<LangData>>();

    private Dictionary<string, string> cachedData = new Dictionary<string, string>();

    public UnityEvent<string> OnLangChanged = new UnityEvent<string>();

    protected void Start()
    {
#if !UNITY_ANDROID
        var ExternalLangDirectory = Application.streamingAssetsPath + "/Langs";
        if (!Directory.Exists(ExternalLangDirectory))
        {
            Directory.CreateDirectory(ExternalLangDirectory);
        }

        if (Directory.GetFiles(ExternalLangDirectory).Length > 0)
        {
            foreach (var langFile in Directory.GetFiles(ExternalLangDirectory))
            {
                FileInfo fileInfo = new FileInfo(langFile);
                if (fileInfo.Extension == ".json")
                {
                    LoadLangFile(langFile);
                }
            }
        }
#else
        foreach (var langFile in StreamingAssetsHelper.ListFiles("Langs"))
        {
            FileInfo fileInfo = new FileInfo(langFile);
            if (fileInfo.Extension == ".json")
            {
                LoadLangFile(langFile);
            }
        }
#endif
        var systemLang = "en";
        try
        {
            systemLang = LauncherSystemHandler.Instance.GetLanguage().ToString().Substring(0, 2);
            if (!loadedLanguages.ContainsKey(systemLang))
            {
                systemLang = "en";
            }
        }
        catch (Exception e)
        {
            Debug.LogError("There was a error getting the system language defaulting to english");
            systemLang = "en";
        }
        ChangeLang(systemLang);
    }

    /// <summary>
    /// Load's a json language file into the memory
    /// </summary>
    /// <param name="filePath">The file to load</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void LoadLangFile(string filePath)
    {
        JObject LoadedObject = JObject.Parse(StreamingAssetsHelper.ReadAllText(filePath));

        string sl = LoadedObject.GetValue("lang").Value<string>();
        List<LangData> entryList = new List<LangData>();

        if (!loadedLanguages.ContainsKey(sl))
        {
            foreach (var data in LoadedObject.GetValue("data").Value<JObject>().Properties())
            {
                try
                {
                    if (data.Type == JTokenType.Property)
                    {
                        if (data.Value.Type == JTokenType.Object)
                        {
                            JObject parsed_obj = (JObject)data.Value;
                            entryList.Add(new LangData()
                            {
                                Id = data.Name,
                                StringData = new LanguageEntry<string>(data.Value.ToString()),
                                JsonData = new LanguageEntry<JObject>(parsed_obj)
                            });
                        }
                        else if (data.Value.Type == JTokenType.Array)
                        {
                            JArray parsed_array = (JArray)data.Value;
                            var id = data.Name;
                            object dat = null;
                            ProcessConditionalEntry<JArray>(parsed_array, ref dat);
                            entryList.Add(new LangData()
                            {
                                Id = id,
                                StringData = new LanguageEntry<string>(dat?.ToString()),
                                conditional = true,
                                JsonArrayData = new LanguageEntry<JArray>(parsed_array)

                            });
                        }
                        else if (data.Value.Type == JTokenType.String)
                        {
                            entryList.Add(new LangData()
                            {
                                Id = data.Name,
                                StringData = new LanguageEntry<string>(data.Value.ToString())
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            loadedLanguages.Add(sl, entryList);
        }
    }

    /// <summary>
    /// Get's a language entry in the active languages jason file
    /// </summary>
    /// <param name="id">The id of the requested entry</param>
    /// <returns>The found entry</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public string GetEntry(string id)
    {
        try
        {
            if (cachedData.ContainsKey(id.RemoveSpecialCharacters()))
            {
                return cachedData[id.RemoveSpecialCharacters()];
            }

            foreach (var entry in ActiveLangEntryList())
            {
                if (entry.Id.Equals(id.RemoveSpecialCharacters()))
                {
                    cachedData.Add(id.RemoveSpecialCharacters(), entry.StringData.Value);
                    return entry.StringData.Value;
                }
            }
        }
        catch (Exception)
        {

        }

        return id.ToUpper();
    }

    /// <summary>
    /// Get's the MultiData entry in the active languages json file
    /// </summary>
    /// <param name="id">The id of the entry</param>
    /// <returns>Th found entry</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public JObject GetMultiDataEntry(string id)
    {
        try
        {
            foreach (var entry in ActiveLangEntryList())
            {
                if (entry.Id.Equals(id.RemoveSpecialCharacters()))
                {
                    return (JObject)entry.JsonData.Value;
                }
            }
        }
        catch (Exception)
        {

        }

        return JObject.Parse($"{{ \"id\": \"{id.RemoveSpecialCharacters().ToUpper()}\"}}");
    }

    /// <summary>
    /// Get's the load lang data out of the memory for the systems active language
    /// </summary>
    /// <returns>A list of the lang data if it's found</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public List<LangData> ActiveLangEntryList()
    {
        foreach (var loadedLangs in loadedLanguages)
        {
            if (loadedLangs.Key == ActiveApplicationLang)
            {
                return loadedLangs.Value;
            }
        }
        return null;
    }

    public void RefreshConditionals()
    {
        foreach (LangData dat in loadedLanguages[LangID])
        {
            if (dat.conditional)
            {
                RefreshConditional(dat);
            }
        }
    }

    public void RefreshConditional(LangData data)
    {
        object dat = null;
        ProcessConditionalEntry<JArray>(data.JsonArrayData.Value, ref dat);
        data.StringData = new LanguageEntry<string>(dat?.ToString());
    }

    /// <summary>
    /// Change's the currently active language
    /// </summary>
    /// <param name="langID">The id of the new language</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void ChangeLang(string langID)
    {
        if (!ActiveApplicationLang.Equals(langID))
        {
            if (loadedLanguages.ContainsKey(langID))
            {
                InvokeLangChangeEvents(langID);
            }
            else
            {
                InvokeLangChangeEvents("English");
            }
        }
    }

    /// <summary>
    /// Invokes All of the events and function needed for a lang change
    /// </summary>
    /// <remarks>Helper function for <see cref="ChangeLang(string)"/></remarks>
    /// <param name="langID">The lang to change to</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    private void InvokeLangChangeEvents(string langID)
    {
        cachedData.Clear();
        ActiveApplicationLang = langID;
        OnLangChanged.Invoke(langID);
    }

    void ProcessConditionalEntry<J>(J data, ref object dat) where J : JToken
    {
        if (data.Type == JTokenType.Object)
        {

            Type loaded_condition_class = Type.GetType(data["Condition_class"].ToString());
            MethodInfo method = loaded_condition_class.GetMethod(data["Test_Condition_func_name"].ToString());
            bool result = (bool)method.Invoke(null, new object[] { });

            if (result)
            {
                if (data["OnTrue"] != null)
                {
                    JToken onTrue = data["OnTrue"];
                    if (onTrue.Type == JTokenType.Object || onTrue.Type == JTokenType.Array)
                    {
                        ProcessConditionalEntry<JObject>((JObject)onTrue, ref dat);
                    }
                    else
                    {
                        dat = onTrue.ToString();
                    }
                }
            }
            else
            {
                if (data["OnFalse"] != null)
                {
                    JToken onFalse = data["OnFalse"];
                    if (onFalse.Type == JTokenType.Object || onFalse.Type == JTokenType.Array)
                    {
                        ProcessConditionalEntry<JObject>((JObject)onFalse, ref dat);
                    }
                    else
                    {
                        dat = onFalse.ToString();
                    }
                }
            }
        }
        else if (data.Type == JTokenType.Array)
        {
            foreach (var conditionalEntry in data.Children<JObject>())
            {
                var conditionClassToken = conditionalEntry.Property("Condition_class");
                var testConditionFuncNameToken = conditionalEntry.Property("Test_Condition_func_name");
                if (conditionClassToken != null && testConditionFuncNameToken != null)
                {
                    Type loadedConditionClass = Type.GetType(conditionClassToken.Value.ToString());
                    MethodInfo method = loadedConditionClass.GetMethod(testConditionFuncNameToken.Value.ToString());
                    bool conditionResult = (bool)method.Invoke(null, new object[] { });

                    if (conditionResult)
                    {
                        var onTrueToken = conditionalEntry.Property("OnTrue");
                        if (onTrueToken != null)
                        {
                            ProcessConditionalEntry<JObject>((JObject)onTrueToken.Value, ref dat);
                            break;
                        }
                    }
                    else
                    {
                        var onFalseToken = conditionalEntry.Property("OnFalse");
                        if (onFalseToken != null)
                        {
                            ProcessConditionalEntry<JObject>((JObject)onFalseToken.Value, ref dat);
                            break;
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public struct LanguageEntry<t>
    {
        public t Value;

        public LanguageEntry(t value)
        {
            Value = value;
        }
    }

    [Serializable]
    public struct LangData
    {
        public string Id;
        public bool conditional;
        public LanguageEntry<JObject> JsonData;
        public LanguageEntry<JArray> JsonArrayData;
        public LanguageEntry<string> StringData;
    }
}
