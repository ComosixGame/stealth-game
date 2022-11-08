using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool readyAttack;
    public event Action<bool> OnAttack;


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
        scanner.Scan();
    }

    private void HandleNotDetectedTarget() {
        OnAttack?.Invoke(false);
        readyAttack = false;
        rigAimLayer.weight = rigAimLayer.weight = Mathf.Lerp(rigAimLayer.weight, -0.1f, 20f * Time.deltaTime);
    }

    private void HandleDetectedTarget(List<RaycastHit> listHit) {
        OnAttack?.Invoke(true);
        Transform hitTransform = scanner.DetectSingleTarget(listHit);
        Vector3 dirLook = hitTransform.position - transform.position;
        dirLook.y = 0;
        transform.rotation = Quaternion.LookRotation(dirLook.normalized);
        rigAimLayer.weight = Mathf.Lerp(rigAimLayer.weight, 1.1f, 20f * Time.deltaTime);
        if(rigAimLayer.weight == 1 && !readyAttack) {
            StartCoroutine(WaitForReadyAttack());
        }
        if(readyAttack) {
            playerWeapon.Attack(hitTransform, scanner.layerMaskTarget, "FromPlayer");
        }
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
    }

    IEnumerator WaitForReadyAttack() {
        yield return new WaitForSeconds(0.1f);
        readyAttack = true;
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(rootScanner != null) {
            scanner.EditorGizmo(rootScanner, angel, range);
        }
    }
    #endif
}
