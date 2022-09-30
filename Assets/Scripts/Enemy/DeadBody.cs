using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public bool isDetected;
    private Rigidbody rb;
    private Collider colliderComp;
    
    private void Awake() {
        colliderComp = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDetected) {
            Destroy(colliderComp);
        }
    }
}
