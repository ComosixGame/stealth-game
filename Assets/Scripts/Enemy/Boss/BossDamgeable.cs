using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BossDamgeable : MonoBehaviour, Damageable
{
    public AudioClip audioClip, deathAudioClip;
    [Range(0,1)] public float volumeScale;
    public GameObject DestroyedBody, healthbar;
    public Slider sliderHealthbar;
    Rigidbody[] ragdollRigibodies;
    public float coinBonus;
    public float health;
    private bool isDead;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;

    private void Awake() {
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;
        healthbar.SetActive(true);        
    }

    private void Start() {
        sliderHealthbar.maxValue = health;
        sliderHealthbar.value = health;
    }

    public void TakeDamge(Vector3 hitPoint, Vector3 force, float damage = 0)
    {
        soundManager.PlayOneShot(audioClip, volumeScale);
        Quaternion rot = Quaternion.LookRotation(-force);
        objectPooler.SpawnObject("HitEffect",hitPoint,rot);
        health -= damage;
        sliderHealthbar.value = health;
        if(health <= 0 && !isDead) {
            isDead = true;
            soundManager.PlayOneShot(deathAudioClip,volumeScale);
            // GameObject weapon = gameObject.GetComponent<EnemyBehaviourScript>().weapon;
            //phá hủy gameobject hiện tại và thay thế bằng ragdoll
            Destroy(gameObject);

            // rớt tiền thưởng
            while(coinBonus > 0) {
                objectPooler.SpawnObject("Money", transform.position, transform.rotation);
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

    private Rigidbody getHitRigi(Rigidbody[]ragdollRigibodies, Vector3 hitPoint) {
        Rigidbody hitRigi =
            ragdollRigibodies
                .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
                .First(rb => rb.gameObject.layer != LayerMask.NameToLayer("Weapon"));
        return hitRigi;
    }

}
