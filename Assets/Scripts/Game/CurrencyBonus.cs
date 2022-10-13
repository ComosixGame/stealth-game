using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CurrencyBonus : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private int point;
    private Rigidbody rb;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector3 dir = Random.insideUnitSphere.normalized;
        rb.AddForce(dir * 8f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other) {
        if((layer & (1 << other.gameObject.layer)) != 0) {
            gameManager.UpdateCurrency(point);
            Destroy(gameObject);
        } 
    }


}
