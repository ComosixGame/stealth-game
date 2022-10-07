using System.Linq;
using UnityEngine;

public class PlayerDamageable : MonoBehaviour, Damageable
{
    [SerializeField] private float health;
    public GameObject DestroyedBody;
    Rigidbody[] ragdollRigibodies;
    private void Awake() {
        GameManager.Instance.UpdatePlayerHealth(health);
    }

    public  void TakeDamge(Vector3 hitPoint , Vector3 force, float damage)
    {   
        health -= damage;
        GameManager.Instance.UpdatePlayerHealth(health);
        if(health == 0) {
            GameObject weapon = gameObject.GetComponent<PlayerAttack>().weapon;

            //phá hủy gameobject hiện tại và thay thế bằng ragdoll
            Destroy(gameObject);
            GameObject deadBody = Instantiate(DestroyedBody, transform.position, transform.rotation);

            // thêm súng của nhân vật vào ragdoll
            Transform gunHolder =  deadBody.transform.Find("GunHolder").transform;
            GameObject w = Instantiate(weapon, gunHolder.position, gunHolder.rotation);
            w.transform.SetParent(gunHolder);
            // thêm rigi body vào súng để có hiệu ứng vật lý
            Rigidbody rigidbodyWeapon = w.AddComponent<Rigidbody>();

            //thêm lực vào bộ phận gần vị trí trúng nhất
            ragdollRigibodies = deadBody.GetComponentsInChildren<Rigidbody>();
            Rigidbody hitRigi = ragdollRigibodies.OrderBy(rb => Vector3.Distance(rb.position, hitPoint)).First();
            hitRigi.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
            //thêm lực văng vào súng
            rigidbodyWeapon.AddForce(force * 5f, ForceMode.Impulse);
        }
    }

    public void TakeDamge(Vector3 hitPoint, Vector3 force)
    {
        throw new System.NotImplementedException();
    }
}
