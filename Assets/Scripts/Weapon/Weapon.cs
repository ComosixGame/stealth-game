using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    public float force;
    public AudioClip audioClip;
    [SerializeField] protected float damage, delayAttack;
    [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
    protected float timeNextAttack;
    private Animator _animator;
    private int attackHash;
    public UnityEvent OnAttack;
    public abstract void Attack(Transform target, LayerMask targets, String namelayerMask);

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
    
}
