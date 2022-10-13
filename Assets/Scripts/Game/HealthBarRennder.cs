using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
#if UNITY_EDITOR
[CanEditMultipleObjects]
#endif
public class HealthBarRennder
{
    public GameObject healthBar;
    public Transform healthBarHolder;
    public float offset;
    private GameObject _healthBar;
    private Slider sliderHealthBar;

    public GameObject CreateHealthBar(float Maxhealth) {
        _healthBar = GameObject.Instantiate(healthBar);
        _healthBar.transform.SetParent(healthBarHolder);
        sliderHealthBar = _healthBar.GetComponent<Slider>();
        sliderHealthBar.maxValue = Maxhealth;
        sliderHealthBar.value = Maxhealth;
        return _healthBar;
    }

    public void UpdateHealthBarPosition(Vector3 position)
    {
        _healthBar.transform.position = Camera.main.WorldToScreenPoint(position + Vector3.up * offset);
    }

    public void UpdateHealthBarValue(float health) {
        sliderHealthBar.value = health;
    }

}
