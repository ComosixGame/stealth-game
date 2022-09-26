using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable
{

    private void Awake() {
        GameManager.Instance.UpdatePlayerHealth(health);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void TakeDamge(Vector3 hitPoint ,float damage, float force)
    {   if(health != 0) {
            health -= damage;
            GameManager.Instance.UpdatePlayerHealth(health);
        } else {
            Destroy(gameObject);
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();
            Rigidbody rb = deadBody.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(dirForce * force, hitPoint, ForceMode.VelocityChange);
        }
    }
}
