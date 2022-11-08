using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CurrencyBonus : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private int point;
    public AudioClip audioClip;
    public float volumeScale = 1;
    private Rigidbody rb;
    private GameManager gameManager;
    private SoundManager soundManager;
    private MoneyPooler moneyPooler;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        moneyPooler = MoneyPooler.Instance;
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    private void OnEnable() {
        Vector3 dir = Random.insideUnitSphere.normalized;
        rb.AddForce(dir * 8f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other) {
        if((layer & (1 << other.gameObject.layer)) != 0) {
            moneyPooler.InactiveMoney(gameObject);
            soundManager.PlayOneShot(audioClip, volumeScale);
        } 
    }

    private void OnDisable() {
        gameManager.UpdateCurrency(point);
    }
}
