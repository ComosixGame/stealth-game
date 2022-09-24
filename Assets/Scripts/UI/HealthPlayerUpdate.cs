using UnityEngine;
using TMPro;

public class HealthPlayerUpdate : MonoBehaviour
{
    private TextMeshProUGUI text;
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnUpdateHealthPlayer.AddListener(UpdateTextHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTextHealth(float health) {
        text.text = health.ToString();
    }

    private void OnDisable() {
        GameManager.Instance.OnUpdateHealthPlayer.RemoveListener(UpdateTextHealth);
    }
}
