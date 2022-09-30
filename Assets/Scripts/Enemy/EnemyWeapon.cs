using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour {
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    public abstract void Attack(Transform TargetTransform, LayerMask targets);
    
}