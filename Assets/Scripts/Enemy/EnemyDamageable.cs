using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDamageable : MonoBehaviour, IDamageable
{
    public AudioClip audioClip, deathAudioClip;
    [Range(0,1)] public float volumeScale;
    public GameObject DestroyedBody;
    Rigidbody[] ragdollRigibodies;
    private float _coinBonus;
    private float _health;
    [SerializeField] private HealthBarRennder healthBarRennder = new HealthBarRennder();
    private bool isDead;
    public UnityEvent<Vector3> OnTakeDamge;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;

    private void Awake() {
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;
    }

    private void LateUpdate() {
        healthBarRennder.UpdateHealthBarRotation();
    }


    public void TakeDamge(Vector3 hitPoint,Vector3 force, float damage)
    {
        soundManager.PlayOneShot(audioClip, volumeScale);
        Quaternion rot = Quaternion.LookRotation(-force);
        objectPooler.SpawnObject("HitEffect",hitPoint,rot);
        _health -= damage;
        healthBarRennder.UpdateHealthBarValue(_health);
        OnTakeDamge?.Invoke(force);
        if(_health <= 0 && !isDead) {
            isDead = true;
            soundManager.PlayOneShot(deathAudioClip,volumeScale);
            GameObject weapon = gameObject.GetComponent<EnemyBehaviourScript>().weapon;
            //phá hủy gameobject hiện tại và thay thế bằng ragdoll
            Destroy(gameObject);

            // rớt tiền thưởng
            while(_coinBonus > 0) {
                objectPooler.SpawnObject("Money", transform.position, transform.rotation);
                _coinBonus--;
            }
        
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            

            // thêm súng của nhân vật vào ragdoll
            Transform gunHolder =  deadBody.transform.Find("GunHolder").transform;
            GameObject w = Instantiate(weapon, gunHolder.position, gunHolder.rotation);
            w.transform.SetParent(gunHolder);
            // thêm rigi body vào súng để có hiệu ứng vật lý
            Rigidbody rigidbodyWeapon = w.AddComponent<Rigidbody>();

            //thêm lực vào bộ phận gần vị trí trúng nhất
            ragdollRigibodies = deadBody.GetComponentsInChildren<Rigidbody>();
            Rigidbody hitRigi = getHitRigi(ragdollRigibodies, hitPoint);
            hitRigi.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            //thêm lực văng vào súng
            rigidbodyWeapon.AddForce(force.normalized * 5f, ForceMode.Impulse);
            
        }
    }

    private Rigidbody getHitRigi(Rigidbody[]ragdollRigibodies, Vector3 hitPoint) {
        Rigidbody hitRigi =
            ragdollRigibodies
                .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
                .First(rb => rb.gameObject.layer != LayerMask.NameToLayer("Weapon"));
        return hitRigi;
    }

    public void setInit(float health, float coinBonus) {
        _health = health;
        _coinBonus = coinBonus;
        healthBarRennder.CreateHealthBar(transform, health);
    }
}
