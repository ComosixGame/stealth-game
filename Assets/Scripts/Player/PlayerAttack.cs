using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerWeapon playerWeapon;
    [SerializeField] private Scanner scanner = new Scanner();

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(playerWeapon.Attack);
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

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(playerWeapon.Attack);
        scanner.OnNotDetectedTarget.RemoveListener(HandleNotDetectedTarget);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        scanner.EditorGizmo(transform, playerWeapon.angel, playerWeapon.range);
    }
    #endif
}
