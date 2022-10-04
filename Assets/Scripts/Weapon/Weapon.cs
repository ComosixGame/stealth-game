using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    public float force;
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    public UnityEvent OnAttack;
    private Animator _animator;
    public AnimationClip weaponPlayAnimation;
    private AnimatorController controller;
    private AnimatorState weaponPlayState;
    private int attackHash;
    public abstract void Attack(Transform target, LayerMask layerMask);

    protected virtual void Awake() {
        attackHash = Animator.StringToHash("Attack");
    }
    protected virtual void OnEnable() {
        OnAttack.AddListener(WeaponPlayAnimation);
    }

    public void getAnimationWeaponPlay(Animator animator) {
        //set motion animation cho layer weaponPlay
        controller = (AnimatorController)animator.runtimeAnimatorController;
        int indexlayer = animator.GetLayerIndex("WeaponPlay");
        weaponPlayState = controller.layers[indexlayer].stateMachine.states[1].state;
        controller.SetStateEffectiveMotion(weaponPlayState, weaponPlayAnimation);
        _animator = animator;
    }

    private void WeaponPlayAnimation() {
        _animator.SetTrigger(attackHash);
    }

    protected virtual void OnDisable() {
        OnAttack.RemoveListener(WeaponPlayAnimation);
    }

    protected virtual void OnDestroy() {
        //remove animaion motion
        if(controller != null){
            controller.SetStateEffectiveMotion(weaponPlayState, null);
        }
    }
    
}
