using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class Cage : MonoBehaviour
{
    public Transform leavePoint;
    public Hostage hostage;
    public LayerMask layer;
    public AudioClip audioClip;
    [SerializeField] private float coinBonus;
    [Range(0, 1)] public float volumeScale;
    public UnityEvent OnEnter;
    private bool opened;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;

    private void Awake() {
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;
    }

    private void OnTriggerEnter(Collider other) {
        if(opened) return;
        if((layer & (1 << other.gameObject.layer)) != 0) {
            soundManager.PlayOneShot(audioClip, volumeScale);
            OnEnter?.Invoke();
            opened = true;
            Invoke("HostageLeave", 1.2f);
            while(coinBonus > 0) {
                objectPooler.SpawnObject("Money", transform.position, transform.rotation);
                coinBonus--;
            }
        }
    }

    private void HostageLeave() {
        hostage.Leave(leavePoint.position);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.magenta;
        Handles.DrawDottedLine(transform.position, leavePoint.position, 5f);
        Handles.DrawSolidDisc(leavePoint.position, Vector3.up, 1f);
    }
#endif
}
