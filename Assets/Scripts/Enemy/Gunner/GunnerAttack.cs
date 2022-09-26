using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerAttack : AttackAction
{
    public Transform gun;
    public GameObject bullet;
    public ParticleSystem shotEffect;
    public float speedBullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Attack(Transform TargetTransform)
    {
        gun.LookAt(TargetTransform.position);
        if(Time.time >= timeNextAttack) {
            GameObject c_bullet = Instantiate(bullet, transform.position,Quaternion.identity);
            shotEffect.Play();
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(transform.forward, speedBullet, damage, 8f);
            timeNextAttack = Time.time + delayAttack;
        }
    }
}
