using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public GameObject impactEffect;
    private Rigidbody bulletRigidbody;
    private Vector3 dir;
    private float speed;
    private bool triggered;
    private float damage;
    private float force;
    private LayerMask layerMask;
    private void Awake() {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if(triggered) {
            FireBullet();
            Destroy(gameObject, 10f);
        }
    }
    
    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
        ContactPoint contact = other.GetContact(0);
        if((layerMask & (1 << other.gameObject.layer)) != 0) {
            Damageable damageable =  other.transform.GetComponentInParent<Damageable>();
            if(damageable != null) {
                damageable.TakeDamge(contact.point, damage, force);
            }
        } else {
            GameObject obj = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            if(obj.GetComponent<ParticleSystem>().isStopped) {
                Destroy(obj);
            }
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            ObstacleDamageable damageable = other.transform.GetComponentInParent<ObstacleDamageable>();
            damageable.TakeDamge(contact.point, 15f);
        }
    }

    private void FireBullet() {
        bulletRigidbody.velocity = dir * speed;
    }

    public void TriggerFireBullet(Vector3 _dir, float _speed, float _damage, float _force, LayerMask _layerMask) {
        dir = _dir;
        speed = _speed;
        damage = _damage;
        force = _force;
        layerMask = _layerMask;
        triggered = true;
    }
}
