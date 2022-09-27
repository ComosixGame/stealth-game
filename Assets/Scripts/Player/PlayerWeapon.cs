using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    [Range(0, 360)] public float angel;
    public float range;
    public AudioClip audioEffect;
    [SerializeField] protected LayerMask targets;
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    public abstract void Attack(Transform TargetTransform);
    public abstract void Idle(Transform _transform);
}