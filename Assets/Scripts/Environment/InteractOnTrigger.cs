using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
[ExecuteInEditMode]
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

    [ExecuteInEditMode]
    // Start is called before the first frame update
    private void OnEnable()
    {
    #if UNITY_EDITOR
        command.SetCommander(transform);
    #endif
    }

    private void OnDisable() {
    #if UNITY_EDITOR
        command.RemoveCommander(transform);
    #endif 
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
        Gizmos.DrawLine(command.transform.position, transform.position);
        Handles.Label(transform.position, "Commander","TextField");
        Handles.Label(command.transform.position, "Interact Object","TextField");
    }

    private void OnDrawGizmos() {

    }
#endif
}
