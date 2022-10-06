using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractOnDestroy : MonoBehaviour
{
    [SerializeField] private Command command;
    public UnityEvent OnObjectDestroy;

    private void OnDestroy() {
        if(command != null) {
            command.Execute();
        }
        OnObjectDestroy?.Invoke();
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
