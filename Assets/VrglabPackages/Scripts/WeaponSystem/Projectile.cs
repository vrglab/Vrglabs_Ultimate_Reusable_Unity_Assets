using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Weapons/Ammo/Projectile")]
public class Projectile : WorldObject
{
    public DataMaping ProjectileDataMaping = new DataMaping()
    {
        floatData = new List<DataMaping.FloatData>() {
        new DataMaping.FloatData("speed", 10f),
        new DataMaping.FloatData("lifespan", 5f),
        new DataMaping.FloatData("knockback", 0f)
        },

        intData = new List<DataMaping.IntData>() {
        new DataMaping.IntData("damage", 1)
        },

        booleanData = new List<DataMaping.BooleanData>() {
        new DataMaping.BooleanData("destroy_on_hit", true),
        new DataMaping.BooleanData("play_impact_vfx", false),
        new DataMaping.BooleanData("play_trail_vfx", false)
        },
    };


    public DataHolder GetDataHolderObject()
    {
        return DataMaping.ParseToDataHolderObject(ProjectileDataMaping);
    }

    public override void OnAddedToInventory(params object[] args)
    {

    }

    public override void OnRemovedToInventory(params object[] args)
    {

    }

    public override void OnUse(params object[] args)
    {

    }

    public override void OnUserInteract()
    {

    }

    public override void OnWorldInteract()
    {

    }
}
