
using UnityEngine;
public abstract class Damageable: MonoBehaviour {
    public GameObject DestroyedBody;
    [SerializeField] protected float health;
    public abstract void TakeDamge(float damage);
}