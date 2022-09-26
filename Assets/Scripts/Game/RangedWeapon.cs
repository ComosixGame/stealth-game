using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeapon : AttackAction
{   public Transform root;
    public GameObject bullet;
    public ParticleSystem shotEffect;
    public float speedBullet;
    public float force;
    public override void Attack(Transform TargetTransform)
    {
        RangedWeaponAttack(TargetTransform);
    }

    public abstract void RangedWeaponAttack(Transform TargetTransform);
}
