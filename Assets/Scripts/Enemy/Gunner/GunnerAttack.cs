using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerAttack : RangedWeapon
{
    public override void RangedWeaponAttack(Transform TargetTransform)
    {
        transform.LookAt(TargetTransform.position);
        if(Time.time >= timeNextAttack) {
            GameObject c_bullet = Instantiate(bullet, root.position,Quaternion.identity);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(root.forward, speedBullet, damage, force);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
