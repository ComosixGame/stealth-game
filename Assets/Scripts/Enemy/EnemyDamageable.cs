using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : Damageable
{
    public override void TakeDamge(Vector3 hitPoint, float damage, float force)
    {
        health -= damage;
        if(health <= 0) {
            Destroy(gameObject);
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();
            Rigidbody rb = deadBody.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(dirForce * force, hitPoint, ForceMode.VelocityChange);
            Destroy(rb, 5f);
            Destroy(deadBody.GetComponent<Collider>(), 5f);
        }
    }
}
