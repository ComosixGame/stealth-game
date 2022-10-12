using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarRennder : MonoBehaviour
{
    public GameObject healthBar;
    public Transform healthBarHolder;
    public float offset;
    private GameObject _healthBar;
    // Start is called before the first frame update
    void Start()
    {
        
        _healthBar = Instantiate(healthBar);
        _healthBar.transform.SetParent(healthBarHolder);
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * offset);
        
    }

    private void LateUpdate() {
    }

    private void OnDestroy() {
        Destroy(_healthBar);
    }
}
