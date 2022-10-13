using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDamageable : MonoBehaviour, Damageable
{
    public GameObject DestroyedBody, Currency;
    Rigidbody[] ragdollRigibodies;
    private float _coinBonus;
    private float _health;
    [SerializeField] private HealthBarRennder healthBarRennder = new HealthBarRennder();
    private GameObject healthBar;
    public UnityEvent<Vector3> OnTakeDamge;


    private void Update() {
        healthBarRennder.UpdateHealthBarPosition(transform.position);
    }

    public void TakeDamge(Vector3 hitPoint,Vector3 force, float damage)
    {
        _health -= damage;
        healthBarRennder.UpdateHealthBarValue(_health);
        OnTakeDamge?.Invoke(force);
        if(_health <= 0) {

            GameObject weapon = gameObject.GetComponent<EnemyBehaviourScript>().weapon;
        
            //phá hủy gameobject hiện tại và thay thế bằng ragdoll
            Destroy(gameObject);
            while(_coinBonus > 0) {
                Instantiate(Currency, transform.position, transform.rotation);
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
        healthBar = healthBarRennder.CreateHealthBar(health);
    }

    private void OnDestroy() {
        Destroy(healthBar);
    }
}
