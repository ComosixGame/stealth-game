using System.Collections.Generic;
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
    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
        
    }
    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.AddListener(HandleNotDetectedTarget);

    }

    private void Start() {
        scanner.CreataFieldOfView(rootScanner, rootScanner.position, angel, range);

        GameObject w = weaponHolder.AddWeapon(weapon);
        playerWeapon = w.GetComponent<Weapon>();
        playerWeapon.getAnimationWeaponPlay(animator);

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

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(rootScanner != null) {
            scanner.EditorGizmo(rootScanner, angel, range);
        }
    }
    #endif
}
