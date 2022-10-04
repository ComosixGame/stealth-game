using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAttack : MonoBehaviour
{
    public GameObject weapon;
    public WeaponHolder weaponHolder;
    private Weapon playerWeapon;
    public Transform rootScanner;
    public Rig rigAimLayer;
    [SerializeField] private float range;
    [SerializeField] [Range(0, 360)] private float angel;
    [SerializeField] private Scanner scanner = new Scanner();
    private int AttackHash;
    private Animator animator;
    private AnimationClip weaponPlayAnimation;
    private AnimatorController controller;
    private AnimatorState weaponPlayState;


    private void Awake() {
        animator = GetComponent<Animator>();
        
    }
    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.AddListener(HandleNotDetectedTarget);

    }

    private void Start() {
        scanner.CreataFieldOfView(rootScanner, rootScanner.position, angel, range);

        AttackHash = Animator.StringToHash("Attack");
        GameObject w = weaponHolder.AddWeapon(weapon);
        playerWeapon = w.GetComponent<Weapon>();
        

        //set motion animation cho layer weaponPlay
        weaponPlayAnimation = playerWeapon.weaponPlayAnimation;
        controller = (AnimatorController)animator.runtimeAnimatorController;
        int indexlayer = animator.GetLayerIndex("WeaponPlay");
        weaponPlayState = controller.layers[indexlayer].stateMachine.states[1].state;
        controller.SetStateEffectiveMotion(weaponPlayState, weaponPlayAnimation);
        
        playerWeapon.OnAttack.AddListener(ChangeStateWeaponPlayAnimation);

    }

    private void Update() {
        scanner.Scan(rootScanner);
    }

    private void HandleNotDetectedTarget() {
        rigAimLayer.weight = rigAimLayer.weight = Mathf.Lerp(rigAimLayer.weight, -0.1f, 0.1f);
    }

    private void HandleDetectedTarget(List<RaycastHit> listHit) {
        Transform hitTransform = scanner.DetectSingleTarget(listHit);
        Vector3 dirLook = hitTransform.position - transform.position;
        dirLook.y = 0;
        transform.rotation = Quaternion.LookRotation(dirLook.normalized);
        rigAimLayer.weight = Mathf.Lerp(rigAimLayer.weight, 1.1f, 0.1f);
        if(rigAimLayer.weight == 1) {
            playerWeapon.Attack(hitTransform, scanner.layerMaskTarget);
        }
    }

    private void ChangeStateWeaponPlayAnimation() {
        animator.SetTrigger(AttackHash);
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
        playerWeapon.OnAttack.RemoveListener(ChangeStateWeaponPlayAnimation);

        //remove animaion motion
        controller.SetStateEffectiveMotion(weaponPlayState, null);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(rootScanner != null) {
            scanner.EditorGizmo(rootScanner, angel, range);
        }
    }
    #endif
}
