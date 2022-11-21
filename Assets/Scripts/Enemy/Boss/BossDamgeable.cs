using System.Linq;
using UnityEngine;

public class BossDamgeable : MonoBehaviour, IDamageable
{
    public Boss boss;
    public AudioClip audioClip, deathAudioClip;
    [Range(0,1)] public float volumeScale;
    public GameObject DestroyedBody;
    [SerializeField] private HealthBarRennder healthBarRennder = new HealthBarRennder();
    private Rigidbody[] ragdollRigibodies;
    private bool isDead;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;
    private int coinBonus;
    private float health;

    private void Awake() {
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;   

        coinBonus = boss.coinBonus;
        health = boss.health;
    }

    private void Start() {
        healthBarRennder.CreateHealthBar(transform, health);
    }

    private void LateUpdate() {
        healthBarRennder.UpdateHealthBarRotation();
    }

    public void TakeDamge(Vector3 hitPoint, Vector3 force, float damage = 0)
    {
        soundManager.PlayOneShot(audioClip, volumeScale);
        Quaternion rot = Quaternion.LookRotation(-force);
        objectPooler.SpawnObject("HitEffect",hitPoint,rot);
        health -= damage;
        healthBarRennder.UpdateHealthBarValue(health);
        if(health <= 0 && !isDead) {
            isDead = true;
            Time.timeScale = 0.3f;
            Invoke("EndSlowMotion", 0.3f);
            soundManager.PlayOneShot(deathAudioClip,volumeScale);
            // GameObject weapon = gameObject.GetComponent<EnemyBehaviourScript>().weapon;
            //phá hủy gameobject hiện tại và thay thế bằng ragdoll
            gameObject.SetActive(false);

            // rớt tiền thưởng
            while(coinBonus > 0) {
                objectPooler.SpawnObject("Money_L", transform.position, transform.rotation);
                coinBonus--;
            }
        
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);
            

            // thêm súng của nhân vật vào ragdoll
            // Transform gunHolder =  deadBody.transform.Find("GunHolder").transform;
            // GameObject w = Instantiate(weapon, gunHolder.position, gunHolder.rotation);
            // w.transform.SetParent(gunHolder);
            // thêm rigi body vào súng để có hiệu ứng vật lý
            // Rigidbody rigidbodyWeapon = w.AddComponent<Rigidbody>();

            //thêm lực vào bộ phận gần vị trí trúng nhất
            ragdollRigibodies = deadBody.GetComponentsInChildren<Rigidbody>();
            Rigidbody hitRigi = getHitRigi(ragdollRigibodies, hitPoint);
            hitRigi.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            //thêm lực văng vào súng
            // rigidbodyWeapon.AddForce(force.normalized * 5f, ForceMode.Impulse);
            
        }
    }

    private void EndSlowMotion() {
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    private Rigidbody getHitRigi(Rigidbody[]ragdollRigibodies, Vector3 hitPoint) {
        Rigidbody hitRigi =
            ragdollRigibodies
                .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
                .First(rb => rb.gameObject.layer != LayerMask.NameToLayer("Weapon"));
        return hitRigi;
    }

}
