using UnityEngine;

public class RangedWeapon : PlayerWeapon
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
            GameObject c_bullet = Instantiate(bullet, root.position, Quaternion.identity);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(root.forward.normalized, speedBullet, damage, force, targets);
            timeNextAttack = Time.time + delayAttack;
        }
    }

    public override void Idle(Transform _transform) {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_transform.forward), 5f * Time.deltaTime);
    }
}
