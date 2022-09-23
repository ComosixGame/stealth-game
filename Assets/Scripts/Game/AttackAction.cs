using UnityEngine;

public abstract class AttackAction : MonoBehaviour {
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    [SerializeField] protected AudioClip audioEffect;
    public abstract void Attack(Transform _transforml);
    
}