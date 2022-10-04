using UnityEngine;


public interface Damageable {
    public void TakeDamge(Vector3 hitPoint, float force);
    public void TakeDamge(Vector3 hitPoint, float force, float damage);
}