using System.Linq;
using UnityEngine;

public class EnemyDamageable : MonoBehaviour, Damageable
{
    [SerializeField] private float health;
    public GameObject DestroyedBody;
    Rigidbody[] ragdollRigibodies;

    public void TakeDamge(Vector3 hitPoint, float damage, float force)
    {
        health -= damage;
        if(health <= 0) {
            //tính hướng tấn công
            Vector3 dirForce = transform.position - hitPoint;
            dirForce.y = 0;
            dirForce.Normalize();

            GameObject weapon = gameObject.GetComponent<EnemyBehaviourScript>().weapon;

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
            Rigidbody hitRigi = getHitRigi(ragdollRigibodies, hitPoint);
            hitRigi.AddForceAtPosition(dirForce * force, hitPoint, ForceMode.Impulse);
            //thêm lực văng vào súng
            rigidbodyWeapon.AddForce(dirForce * 5f, ForceMode.Impulse);
        }
    }

    public void TakeDamge(Vector3 hitPoint, float force)
    {
        throw new System.NotImplementedException();
    }

    private Rigidbody getHitRigi(Rigidbody[]ragdollRigibodies, Vector3 hitPoint) {
        Rigidbody hitRigi =
            ragdollRigibodies
                .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
                .First(rb => rb.gameObject.layer != LayerMask.NameToLayer("Weapon"));
        return hitRigi;
    }
}
