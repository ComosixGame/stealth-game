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
        if(!other.transform.tag.Equals("Player")) {
            GameObject obj = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            if(obj.GetComponent<ParticleSystem>().isStopped) {
                Destroy(obj);
            }
        } else {
            if(other.transform.TryGetComponent(out Damageable damageable)){
                damageable.TakeDamge(contact.point, damage, force);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        
    }

    private void FireBullet() {
        bulletRigidbody.velocity = dir * speed;
    }

    public void TriggerFireBullet(Vector3 _dir, float _speed, float _damage, float _force) {
        dir = _dir;
        speed = _speed;
        damage = _damage;
        force = _force;
        triggered = true;
    }
}
