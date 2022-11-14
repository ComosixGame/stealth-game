using System.Collections;
using UnityEngine;

public class ObstacleDamageable : MonoBehaviour, Damageable
{
    public bool destroyAfterHit;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    private Rigidbody[] rigidbodies;
    private Renderer rd;
    private bool destroyed;
    private float timeDelay;
    private SoundManager soundManager;


    private void Awake() {
        rd = GetComponent<Renderer>();
        soundManager = SoundManager.Instance;
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

    public void TakeDamge(Vector3 hitPoint, Vector3 force, float damage)
    {
        if(!destroyed) {
            soundManager.PlayOneShot(audioClip, volumeScale);
            timeDelay = Time.time + 2;

            foreach(Rigidbody rigidbody in rigidbodies) {
                rigidbody.isKinematic = false;
                float f = force.magnitude / Vector3.Distance(rigidbody.position, hitPoint);
                rigidbody.AddForceAtPosition(force.normalized * f, hitPoint, ForceMode.Impulse);
            }

            destroyed = true;
            if(destroyAfterHit) {
                StartCoroutine(ChangeLayer());
                Destroy(gameObject, 5f);
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
