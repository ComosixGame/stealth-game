using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HostageLeave : MonoBehaviour
{
    public LayerMask layer;

    private void OnTriggerEnter(Collider other) {
        GameObject target = other.gameObject;
        if((layer & (1 << target.layer)) != 0) {
            Destroy(target);
        }
    }
}
