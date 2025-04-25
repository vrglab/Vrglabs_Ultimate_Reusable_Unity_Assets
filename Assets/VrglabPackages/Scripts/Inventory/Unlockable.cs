using FMOD;
using UnityEngine;

[CreateAssetMenu(menuName = "inventory/Base Unlockable")]
public class Unlockable : WorldObject
{

    [SerializeField]
    private string _id;

    public override void OnUse(params object[] args)
    {
        
    }

    public override void OnAddedToInventory(params object[] args)
    {
        DataHolder dataHolder = ((object[])args[4])[0] as DataHolder;

        dataHolder.SetData(_id, UnlockValue());
    }

    public override void OnRemovedToInventory(params object[] args)
    {
        DataHolder dataHolder = ((object[])args[4])[0] as DataHolder;
        dataHolder.SetData(_id, LockValue());
    }

    public virtual object UnlockValue()
    {
        return true;
    }

    public virtual object LockValue()
    {
        return false;
    }

    public override void OnWorldInteract()
    {
        
    }

    public override void OnUserInteract()
    {
        
    }
}
