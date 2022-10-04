using System.Linq;
using UnityEngine;

public class ObstacleDamageable : MonoBehaviour, Damageable
{
    public bool destroyAfterHit;
    private Rigidbody[] rigidbodies;
    private Renderer rd;
    private bool destroyed;
    private float timeDelay;


    private void Awake() {
        rd = GetComponent<Renderer>();
    }

    private void Start() {
        rigidbodies = GetComponentsInChildren<Rigidbody>();

    }

    private void FixedUpdate() {
        if(destroyed && Time.time > timeDelay && destroyAfterHit) {
            foreach(Rigidbody rigidbody in rigidbodies) {
                // rigidbody.isKinematic = true;
                rigidbody.velocity = Vector3.down * 5f;
                Destroy(rigidbody.GetComponent<Collider>());
            }

        }
    }

    public void TakeDamge(Vector3 hitPoint, float force)
    {
        if(!destroyed) {
            timeDelay = Time.time + 5;
            //tính hướng tác động
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();

            Rigidbody hitRigi;

            hitRigi = rigidbodies.OrderBy(rb => Vector3.Distance(rb.position, hitPoint)).First();
            hitRigi.transform.GetComponent<Rigidbody>().AddForceAtPosition(dirForce * force, hitPoint, ForceMode.Impulse);


            destroyed = true;
            if(destroyAfterHit) {
                Destroy(gameObject, 10f);
            }
        }

    }

    public void TakeDamge(Vector3 hitPoint, float force, float damage)
    {
        throw new System.NotImplementedException();
    }
}
