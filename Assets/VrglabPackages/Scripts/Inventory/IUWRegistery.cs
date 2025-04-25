using System.Collections.Generic;
using UnityEngine;

public class IUWRegistery : Instancable<IUWRegistery>
{
    public List<WorldObject> IuwTypes;
    public List<string> IuwIds;

    public WorldObject ResolveIUW(string id)
    {
        int index = IuwIds.IndexOf(id);
        return index >= 0 ? IuwTypes[index] : null;
    }

    public string ResolveId(WorldObject iuw)
    {
        int index = IuwTypes.IndexOf(iuw);
        return index >= 0 ? IuwIds[index] : null;
    }
}
