using UnityEngine;

public class RangeWeapon : Weapon
{
    public GameObject _bullet;
    public Transform shootPositon;
    public ParticleSystem shotEffect;
    public float speedBullet;

    public override void Attack(Transform TargetTransform, LayerMask targets, string namelayerMask)
    {
        if(Time.time >= timeNextAttack) {
            OnAttack?.Invoke();
            GameObject c_bullet = Instantiate(_bullet, shootPositon.position, shootPositon.rotation);
            c_bullet.layer = LayerMask.NameToLayer(namelayerMask);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(shootPositon.forward.normalized, speedBullet, damage, force, targets);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
