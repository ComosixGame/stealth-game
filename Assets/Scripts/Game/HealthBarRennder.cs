using UnityEngine;
using UnityEngine.UI;

public class HealthBarRennder : MonoBehaviour
{
    public GameObject healthBar;
    public Transform healthBarHolder;
    public float offset;
    private GameObject _healthBar;
    private Slider sliderHealthBar;
    // Start is called before the first frame update
    void Awake()
    {
        _healthBar = Instantiate(healthBar);
        _healthBar.transform.SetParent(healthBarHolder);
        sliderHealthBar = _healthBar.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * offset);
        
    }

    public void initHealthBar(float Maxhealth) {
        sliderHealthBar.maxValue = Maxhealth;
        sliderHealthBar.value = Maxhealth;
    }

    public void UpdateHealthBar(float health) {
        sliderHealthBar.value = health;
    }


    private void OnDestroy() {
        Destroy(_healthBar);
    }
}
