using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class InteractOnTrigger : MonoBehaviour
{
    [SerializeField] private Command command;
    public LayerMask layer;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    private void Awake() {
        Collider collider = GetComponent<Collider>();
        if(!collider.isTrigger) {
            collider.isTrigger = true;
        }
            
    }


    private void OnTriggerEnter(Collider other) {
        if((layer & (1<<other.gameObject.layer)) != 0) {
            if(command != null) {
                command.Execute();
            }
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if((layer & (1<<other.gameObject.layer)) != 0) {
            OnExit?.Invoke();
        }
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
