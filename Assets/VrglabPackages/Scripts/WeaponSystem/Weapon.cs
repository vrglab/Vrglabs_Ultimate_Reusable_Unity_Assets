using Ink.Parsed;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Basic Weapon", fileName = "NewBasicWeapon")]
public class Weapon : WorldObject
{

    public DataMaping WeaponDataMaping = new DataMaping()
    {
        intData = new List<DataMaping.IntData>() {
            new DataMaping.IntData("max_magazine_amount", 10),
            new DataMaping.IntData("current_magazine_amount", 10),
            new DataMaping.IntData("burst_count", 1)
        },

        floatData = new List<DataMaping.FloatData>() {
            new DataMaping.FloatData("fire_rate", 0.5f),
            new DataMaping.FloatData("reload_time", 2f),
            new DataMaping.FloatData("range", 10f),
            new DataMaping.FloatData("spread_angle", 0f),
            new DataMaping.FloatData("base_damage", 1f)
        },

        booleanData = new List<DataMaping.BooleanData>() {
            new DataMaping.BooleanData("is_automatic", false),
            new DataMaping.BooleanData("is_dual_wieldable", false),
            new DataMaping.BooleanData("has_unlimited_ammo", false)
        },

        stringData = new List<DataMaping.StringData>() {
            new DataMaping.StringData("projectile_id", "default_projectile"),
            new DataMaping.StringData("weapon_type", "ranged")
        }
    };

    public DataHolder GetDataHolderObject()
    {
        return DataMaping.ParseToDataHolderObject(WeaponDataMaping);
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
