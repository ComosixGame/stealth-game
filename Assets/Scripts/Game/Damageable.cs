using UnityEngine;


public interface Damageable {
    public void TakeDamge(Vector3 hitPoint, Vector3 force);
    public void TakeDamge(Vector3 hitPoint, Vector3 force, float damage);
}