using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour {
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    [SerializeField] protected LayerMask targets;
    protected float timeNextAttack;
    public abstract void Attack(Transform TargetTransform);
    
}