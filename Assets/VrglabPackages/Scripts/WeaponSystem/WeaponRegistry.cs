using System.Collections.Generic;
using UnityEngine;

public class WeaponRegistry : Instancable<WeaponRegistry>
{
    public List<Projectile> projectileTypes;
    public List<string> projectileIds;
    public List<GameObject> projectilePrefabs;

    public Projectile ResolveProjectile(string id)
    {
        int index = projectileIds.IndexOf(id);
        return index >= 0 ? projectileTypes[index] : null;
    }

    public GameObject ResolveProjectilePrefab(string id)
    {
        int index = projectileIds.IndexOf(id);
        return index >= 0 ? projectilePrefabs[index] : null;
    }
}
