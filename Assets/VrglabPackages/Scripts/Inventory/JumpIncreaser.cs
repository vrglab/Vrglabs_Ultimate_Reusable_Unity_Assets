using UnityEngine;

[CreateAssetMenu(fileName = "JumpIncreaser", menuName = "inventory/Jump Increasser")]
public class JumpIncreaser : Unlockable
{

    public int value;

    public override object LockValue()
    {
        return 1;
    }

    public override object UnlockValue()
    {
        return value;
    }
}
