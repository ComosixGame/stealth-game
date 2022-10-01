using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAttack : MonoBehaviour
{
    public Weapon playerWeapon;
    public Transform rootScanner, aimLookAt;
    public Rig aimLayer;
    public Rig bodyAimLayer;
    private float delayAttack = 0.3f;
    [SerializeField] private float range;
    [SerializeField] [Range(0, 360)] private float angel;
    [SerializeField] private Scanner scanner = new Scanner();

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleDetectedTarget);
        scanner.OnNotDetectedTarget.AddListener(HandleNotDetectedTarget);
    }

    private void Start() {
        scanner.CreataFieldOfView(rootScanner, rootScanner.position, angel, range);
    }

    private void Update() {
        scanner.Scan(rootScanner);
    }

    private void HandleNotDetectedTarget() {
        float weight = Time.deltaTime/0.1f;
        aimLayer.weight -= weight;
        bodyAimLayer.weight -= weight;
        delayAttack = delayAttack > 0 ? delayAttack - weight : 0;
    }

    private void HandleDetectedTarget(List<RaycastHit> listHit) {
        Transform hitTransform = scanner.DetectSingleTarget(listHit);
        aimLookAt.position = hitTransform.position;
        float weight = Time.deltaTime/0.1f;
        aimLayer.weight += weight;
        bodyAimLayer.weight += weight;
        delayAttack = delayAttack < 2 ? delayAttack + weight : 2;
        if(delayAttack >= 2f) {
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
