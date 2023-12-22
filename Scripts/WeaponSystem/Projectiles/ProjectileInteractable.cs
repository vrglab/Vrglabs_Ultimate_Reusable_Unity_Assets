using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileInteractable
{
    void OnProjectileHit(Projectiles hitProjectile);
}
