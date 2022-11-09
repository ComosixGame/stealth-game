using UnityEngine;

public class RangeWeapon : Weapon
{
    public Transform shootPositon;
    public ParticleSystem shotEffect;
    public float speedBullet;
    [Range(0,1)] public float volumeScale;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;

    protected override void Awake() {
        base.Awake();
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;
    }

    public override void Attack(Transform TargetTransform, LayerMask targets, string namelayerMask)
    {
        if(Time.time >= timeNextAttack) {
            OnAttack?.Invoke();
            GameObject c_bullet = objectPooler.SpawnObject("Bullet", shootPositon.position, shootPositon.rotation);
            c_bullet.layer = LayerMask.NameToLayer(namelayerMask);
            shotEffect.Play();
            soundManager.PlayOneShot(audioClip, volumeScale);
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(shootPositon.forward.normalized, speedBullet, damage, force, targets);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
