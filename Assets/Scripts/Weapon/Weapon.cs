using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    public float force;
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    public AnimationClip weaponPlayAnimation;
    public UnityEvent OnAttack;
    public abstract void Attack(Transform target, LayerMask layerMask);
    
}
