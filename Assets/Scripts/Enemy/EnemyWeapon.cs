using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour {
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    [SerializeField] protected AudioClip audioEffect;
    public abstract void Attack(Transform TargetTransform);
    
}