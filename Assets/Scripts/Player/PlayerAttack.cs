using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerWeapon playerWeapon;
    [SerializeField] private Scanner scanner = new Scanner();

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.AddListener(HandleNotDetectedTarget);
    }

    private void Start() {
        scanner.CreataFieldOfView(transform, transform.position, playerWeapon.angel, playerWeapon.range);
    }

    private void Update() {
        scanner.Scan(transform);
    }

    private void HandleNotDetectedTarget() {
        playerWeapon.Idle(transform);
    }

    private void HandleDetectedTarget(List<RaycastHit> listHit) {
        Transform hitTransform = scanner.DetectSingleTarget(listHit);
        playerWeapon.Attack(hitTransform);
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        scanner.EditorGizmo(transform, playerWeapon.angel, playerWeapon.range);
    }
    #endif
}
