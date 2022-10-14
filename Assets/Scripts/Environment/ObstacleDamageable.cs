using System.Collections;
using System.Linq;
using UnityEngine;

public class ObstacleDamageable : MonoBehaviour, Damageable
{
    public bool destroyAfterHit;
    public AudioClip audioClip;
    public float volumeScale;
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
                Destroy(rigidbody.GetComponent<Collider>());
            }
        }
    }

    public void TakeDamge(Vector3 hitPoint, Vector3 force, float damage = 0)
    {
        if(!destroyed) {
            timeDelay = Time.time + 5;
            //tính hướng tác động
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();

            foreach(Rigidbody rigidbody in rigidbodies) {
                float f = force.magnitude / Vector3.Distance(rigidbody.position, hitPoint);
                rigidbody.AddForceAtPosition(force.normalized * f, hitPoint, ForceMode.Impulse);
            }

            destroyed = true;
            if(destroyAfterHit) {
                StartCoroutine(ChangeLayer());
                Destroy(gameObject, 10f);
            }
        }

    }

    IEnumerator ChangeLayer() {
        yield return new WaitForSeconds(0.3f);
        Transform[] children = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach(Transform child in children) {
            child.gameObject.layer = LayerMask.NameToLayer("BrokenThing");
        }
    }
}
