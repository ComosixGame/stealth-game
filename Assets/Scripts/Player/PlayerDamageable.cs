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
            GameObject weapon = gameObject.GetComponent<PlayerAttack>().weapon;
            Destroy(gameObject);
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            Transform gunHolder =  deadBody.transform.Find("GunHolder").transform;
            GameObject w = Instantiate(weapon, gunHolder.position, gunHolder.rotation);
            w.transform.SetParent(gunHolder);
            Rigidbody rigidbodyWeapon = w.AddComponent<Rigidbody>();
            ragdollRigibodies = deadBody.GetComponentsInChildren<Rigidbody>();
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();
            Rigidbody hitRigi = ragdollRigibodies.OrderBy(rb => Vector3.Distance(rb.position, hitPoint)).First();
            hitRigi.AddForceAtPosition(dirForce * force, hitPoint, ForceMode.Impulse);
            rigidbodyWeapon.AddForce(dirForce * 5f, ForceMode.Impulse);
        }
    }
}
