using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGate : Command
{
    public float damage;
    public float TimeCharge;
    public LayerMask layer;
    private bool ready, turnOff;
    private  float timeNextAttack;

    private void OnTriggerStay(Collider other) {
        if(turnOff) {
            return;
        }
        
        if((layer & (1 << other.gameObject.layer)) != 0 ) {
            Vector3  dir = other.transform.position - transform.position;
            dir.y = 0;

            if(Time.time >= timeNextAttack) {
                ready = true;
            }

            if(ready) {
                other.GetComponent<Damageable>().TakeDamge(transform.position, dir * 20, damage);
                timeNextAttack = Time.time + TimeCharge;
                ready = false;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        ready = true;
        timeNextAttack = 0;
    }
    
    public override void Execute()
    {
        turnOff = true;
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }
    
}
