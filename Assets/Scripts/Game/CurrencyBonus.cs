using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CurrencyBonus : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private int point;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other) {
        if((layer & (1 << other.gameObject.layer)) != 0) {
            gameManager.UpdateCurrency(point);
            Destroy(gameObject);
        } 
    }
}
