using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

[Serializable]
public class DataMaping
{
    public List<StringData> stringData = new List<StringData>();
    public List<IntData> intData = new List<IntData>();
    public List<FloatData> floatData = new List<FloatData>();
    public List<BooleanData> booleanData = new List<BooleanData>();
    public List<Vector3Data> vector3Data = new List<Vector3Data>();
    public List<Vector2Data> vector2Data = new List<Vector2Data>();
    public List<SpriteData> spriteData = new List<SpriteData>();


    public T ResolveData<T>(string key)
    {
        var listFields = GetType().GetFields(BindingFlags.Instance |
                                             BindingFlags.Public |
                                             BindingFlags.NonPublic)
                                   .Where(f => f.FieldType.IsGenericType &&
                                               f.FieldType.GetGenericTypeDefinition() == typeof(List<>));

        foreach (var field in listFields)
        {
            var elementType = field.FieldType.GetGenericArguments()[0];

            Type dataBase = elementType;
            while (dataBase != null &&
                  (!dataBase.IsGenericType ||
                   dataBase.GetGenericTypeDefinition() != typeof(Data<>)))
            {
                dataBase = dataBase.BaseType;
            }
            if (dataBase == null)
                continue;

            var payloadType = dataBase.GetGenericArguments()[0];
            if (payloadType != typeof(T))
                continue;

            var keyField = dataBase.GetField("key");
            var valueField = dataBase.GetField("value");

            IEnumerable list = (IEnumerable)field.GetValue(this);
            foreach (var item in list)
            {
                if ((string)keyField.GetValue(item) == key)
                    return (T)valueField.GetValue(item);
            }
        }

        throw new KeyNotFoundException(
            $"No entry of type '{typeof(T).Name}' with key '{key}' was found.");
    }


    public static DataHolder ParseToDataHolderObject(DataMaping DataMaping)
    {
        DataHolder dataHolder = new DataHolder();
        foreach (var item in DataMaping.intData)
        {
            dataHolder.SetData(item.key, item.value);
        }
        foreach (var item in DataMaping.booleanData)
        {
            dataHolder.SetData(item.key, item.value);
        }
        foreach (var item in DataMaping.floatData)
        {
            dataHolder.SetData(item.key, item.value);
        }
        foreach (var item in DataMaping.stringData)
        {
            dataHolder.SetData(item.key, item.value);
        }
        foreach (var item in DataMaping.vector3Data)
        {
            dataHolder.SetData(item.key, new DataHolder().SerializeVector3(item.value));
        }
        foreach (var item in DataMaping.vector2Data)
        {
            dataHolder.SetData(item.key, new DataHolder().SerializeVector2(item.value));
        }
        return dataHolder;
    }


    #region Data Classes

    [Serializable]
    public class Data <T>
    {
        public string key;
        public T value;

        public Data(string key, T Value)
        {
            this.key = key;
            this.value = Value;
        }
    }

    [Serializable]
    public class IntData : Data<int>
    {
        public IntData(string key, int Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class BooleanData : Data<bool>
    {
        public BooleanData(string key, bool Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class FloatData : Data<float>
    {
        public FloatData(string key, float Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class StringData : Data<String>
    {
        public StringData(string key, string Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class Vector3Data : Data<Vector3>
    {
        public Vector3Data(string key, Vector3 Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class Vector2Data : Data<Vector2>
    {
        public Vector2Data(string key, Vector2 Value) : base(key, Value)
        {
        }
    }

    [Serializable]
    public class SpriteData : Data<Sprite>
    {
        public SpriteData(string key, Sprite Value) : base(key, Value)
        {
        }
    }

    #endregion
}