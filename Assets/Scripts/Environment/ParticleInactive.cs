using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleInactive : MonoBehaviour
{
    public string key;
    private float duration;
    private ParticleSystem particle;
    private ObjectPooler objectPooler;

    private void Awake() {
        objectPooler = ObjectPooler.Instance;
        particle = GetComponent<ParticleSystem>();
        duration = particle.main.duration;
    }

    private void OnEnable() {
        Invoke("InactiveEffect", duration);
    }

    // Start is called before the first frame update


    private void InactiveEffect() {
        objectPooler.InactiveObject(key, gameObject);
    }
}
