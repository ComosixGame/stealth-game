using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractOnDestroy : MonoBehaviour
{
    [SerializeField] private Command command;
    public UnityEvent OnObjectDestroy;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale = 1;
    private bool isEnd;
    private SoundManager soundManager;
    private GameManager gameManager;

    private void Awake() {
        soundManager = SoundManager.Instance;
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnEndGame.AddListener(OnEndGame);
    }

    private void OnDisable() {
        gameManager.OnEndGame.RemoveListener(OnEndGame);
    }

    private void OnDestroy() {
        if(isEnd) return;
        if(command != null) {
            soundManager.PlayOneShot(audioClip, volumeScale);
            command.Execute();
        }
        OnObjectDestroy?.Invoke();
    }

    private void OnEndGame(bool win) {
        isEnd = true;
    }

    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Handles.Label(transform.position, "Commander","TextField");
        if(command != null) {
            Gizmos.DrawLine(command.transform.position, transform.position);
            Handles.Label(command.transform.position, "Interact Object","TextField");
        }
    }

    private void OnDrawGizmos() {
        Handles.color = Color.blue;
        if(command != null) {
            Handles.DrawDottedLine(transform.position, command.transform.position, 3f);
        }
        
    }
#endif
}
