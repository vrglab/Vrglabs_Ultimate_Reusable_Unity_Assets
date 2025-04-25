using UnityEngine;

public abstract class LevelInstanceHandler : Instancable<LevelInstanceHandler>
{
    public abstract void OnLevelLoaded(DataHolder LevelData);
    public abstract void OnLevelUnloaded();
}
