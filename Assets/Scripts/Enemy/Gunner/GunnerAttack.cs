using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerAttack : AttackAction
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Attack(Transform _transform)
    {
        Debug.Log(_transform.tag);
    }
}
