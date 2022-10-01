using System.Linq;
using UnityEngine;

public class PlayerDamageable : Damageable
{
    Rigidbody[] ragdollRigibodies;

    private void Awake() {
        GameManager.Instance.UpdatePlayerHealth(health);
    }

    public override void TakeDamge(Vector3 hitPoint ,float damage, float force)
    {   
        health -= damage;
        GameManager.Instance.UpdatePlayerHealth(health);
        if(health == 0) {
            Destroy(gameObject);
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            ragdollRigibodies = deadBody.GetComponentsInChildren<Rigidbody>();
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();
            Rigidbody hitRigi = ragdollRigibodies.OrderBy(rb => Vector3.Distance(rb.position, hitPoint)).First();
            hitRigi.AddForceAtPosition(dirForce * force, hitPoint, ForceMode.Impulse);
        }
    }
}
