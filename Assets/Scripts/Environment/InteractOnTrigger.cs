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
    public Command command;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    private void Awake() {
        Collider collider = GetComponent<Collider>();
        if(!collider.isTrigger) {
            collider.isTrigger = true;
        }
            
    }


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            command.Execute();
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
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
        Handles.DrawDottedLine(transform.position, command.transform.position, 3f);
    }
#endif
}
