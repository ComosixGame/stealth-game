using UnityEngine;

public class RangedWeapon : Weapon
{
    public Transform shootPositon;
    public GameObject bullet;
    public ParticleSystem shotEffect;
    public float speedBullet;

    public override void Attack(Transform TargetTransform, LayerMask targets)
    {
        if(Time.time >= timeNextAttack) {
            GameObject c_bullet = Instantiate(bullet, shootPositon.position, transform.rotation);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(shootPositon.forward.normalized, speedBullet, damage, force, targets);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
