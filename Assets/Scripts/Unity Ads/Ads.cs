using UnityEngine;
using UnityEngine.Events;

public class Ads : MonoBehaviour
{
    private GameManager gameManager;
    public UnityEvent onEndGame;
    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnEndGame.AddListener(onEnd);
    }

    private void onEnd(bool win) {
        onEndGame?.Invoke();
    }

    private void OnDisable() {
        gameManager.OnEndGame.RemoveListener(onEnd);
    }
}
