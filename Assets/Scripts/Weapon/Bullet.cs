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
        if(!other.transform.tag.Equals("Player")) {
            ContactPoint contact = other.GetContact(0);
            GameObject obj = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            if(obj.GetComponent<ParticleSystem>().isStopped) {
                Destroy(obj);
            }
        } else {
            GameManager.Instance.PlayerGetDamage(damage);
        }
    }

    private void OnTriggerEnter(Collider other) {
        
    }

    private void FireBullet() {
        bulletRigidbody.velocity = dir * speed;
    }

    public void TriggerFireBullet(Vector3 _dir, float _speed, float _damage) {
        dir = _dir;
        speed = _speed;
        damage = _damage;
        triggered = true;
    }
}
