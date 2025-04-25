using UnityEngine;

public sealed class DataIds
{
    #region Creature Keys
    public static string creature_core { get; } = "core_data";
    public static string creature_position { get; } = "position";


    public static string getCreatureCoreDataKey(string creatureId)
    {
        return creatureId + "_" + creature_core;
    }

    public static string getCreaturePositionKey(string creatureId)
    {
        return creatureId + "_" + creature_position ;
    }
    #endregion

    #region Items, Unlocks and Weapons
    public static string iuw_core { get; } = "iuw_core_data";
    public static string iuw_position { get; } = "iuw_position";
    public static string iuw_pickup { get; } = "iuw_pickup";
    public static string iuw_name { get; } = "iuw_name";


    public static string getIUWCoreDataKey(string iuwId)
    {
        return iuwId + "_" + iuw_core;
    }

    public static string getIUWPositionKey(string iuwId)
    {
        return iuwId + "_" + iuw_position;
    }

    public static string getIUWPickupStateKey(string iuwId)
    {
        return iuwId + "_" + iuw_pickup;
    }
    #endregion
}
