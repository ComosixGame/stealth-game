using UnityEngine;

public class GunnerAttack : EnemyWeapon
{
    public Transform root;
    public GameObject bullet;
    public ParticleSystem shotEffect;
    public float speedBullet;
    public float force;
    public override void Attack(Transform TargetTransform)
    {
        transform.LookAt(TargetTransform.position);
        if(Time.time >= timeNextAttack) {
            GameObject c_bullet = Instantiate(bullet, root.position, transform.rotation);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(root.forward.normalized, speedBullet, damage, force, targets);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
