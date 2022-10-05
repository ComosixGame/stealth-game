using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    public float force;
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    [SerializeField] protected RuntimeAnimatorController runtimeAnimatorController;
    protected float timeNextAttack;
    public UnityEvent OnAttack;
    private Animator _animator;
    private int attackHash;
    public abstract void Attack(Transform target, LayerMask layerMask);

    protected virtual void Awake() {
        attackHash = Animator.StringToHash("Attack");
    }
    protected virtual void OnEnable() {
        OnAttack.AddListener(WeaponPlayAnimation);
    }

    public void getAnimationWeaponPlay(Animator animator) {
        _animator = animator;
        _animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    private void WeaponPlayAnimation() {
        _animator.SetTrigger(attackHash);
    }

    protected virtual void OnDisable() {
        OnAttack.RemoveListener(WeaponPlayAnimation);
    }

    protected virtual void OnDestroy() {

    }
    
}
