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
        StartCoroutine(startTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(isDetected && rb.IsSleeping() ) {
            Destroy(colliderComp);
        }
    }

    IEnumerator startTimer() {
        yield return new WaitForSeconds(3);
        rb.isKinematic = true;
    }
}
