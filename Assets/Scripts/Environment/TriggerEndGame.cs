using UnityEngine;

public class TriggerEndGame : MonoBehaviour
{
    public LayerMask layer;
    private GameManager  gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other) {
        if((layer & (1 << other.gameObject.layer)) != 0) {
            gameManager.EndGame(true);
        }
    }
}
