using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable
{

    private void Awake() {
        GameManager.Instance.SetHealthPlayer(health);
    }
    private void OnEnable() {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void TakeDamge(float damage)
    {
        GameManager.Instance.PlayerGetDamage(damage);
    }
}
