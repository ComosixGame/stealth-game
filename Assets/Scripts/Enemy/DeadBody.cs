using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public bool isDetected;
    private Rigidbody rb;
    private Collider colliderComp;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        colliderComp = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.IsSleeping()) {
            rb.isKinematic = true;
            if(isDetected) {
                Destroy(colliderComp);
            }
        }
    }
}
