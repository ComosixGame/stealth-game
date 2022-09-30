using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAttack : MonoBehaviour
{
    public PlayerWeapon playerWeapon;
    public Transform rootScanner, aimLookAt;
    public Rig aimLayer;
    public Rig bodyAimLayer;
    [SerializeField] private Scanner scanner = new Scanner();

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.AddListener(HandleNotDetectedTarget);
    }

    private void Start() {
        scanner.CreataFieldOfView(rootScanner, rootScanner.position, playerWeapon.angel, playerWeapon.range);
    }

    private void Update() {
        scanner.Scan(rootScanner);
    }

    private void HandleNotDetectedTarget() {
        aimLayer.weight -= Time.deltaTime/0.1f;
        bodyAimLayer.weight -= Time.deltaTime/0.1f;
    }

    private void HandleDetectedTarget(List<RaycastHit> listHit) {
        Transform hitTransform = scanner.DetectSingleTarget(listHit);
        aimLayer.weight += Time.deltaTime/0.1f;
        bodyAimLayer.weight += Time.deltaTime/0.1f;
        aimLookAt.position = hitTransform.position;
        if(aimLayer.weight >= 1) {
            playerWeapon.Attack(hitTransform, scanner.layerMaskTarget);
        }
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        scanner.EditorGizmo(rootScanner, playerWeapon.angel, playerWeapon.range);
    }
    #endif
}
