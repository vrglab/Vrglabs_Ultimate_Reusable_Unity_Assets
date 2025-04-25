using Unity.VisualScripting;
using UnityEngine;

public static class DataSerializationUtils
{
    public static DataHolder SerializeVector3(this DataHolder dataHolder, Vector3 vector)
    {
        dataHolder["x"] = vector.x;
        dataHolder["y"] = vector.y;
        dataHolder["z"] = vector.z;

        return dataHolder;
    }

    public static Vector3 ParseToVector3(this DataHolder dataHolder)
    {
        return new Vector3(dataHolder.GetData<float>("x"), dataHolder.GetData<float>("y"), dataHolder.GetData<float>("z"));
    }


    public static DataHolder SerializeVector2(this DataHolder dataHolder, Vector2 vector)
    {
        return SerializeVector3(dataHolder, vector);
    }

    public static Vector2 ParseToVector2(this DataHolder dataHolder)
    {
        return new Vector2(dataHolder.GetData<float>("x"), dataHolder.GetData<float>("y"));
    }
}
