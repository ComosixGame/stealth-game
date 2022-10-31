using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HealthPlayerUpdate : MonoBehaviour
{
    GameManager gameManager;
    private TextMeshProUGUI text;
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        gameManager = GameManager.Instance;
    }
    
    private void OnEnable() {
        gameManager.OnUpdateHealthPlayer.AddListener(UpdateTextHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTextHealth(float health) {
        text.text = health.ToString();
    }

    private void OnDisable() {
        gameManager.OnUpdateHealthPlayer.RemoveListener(UpdateTextHealth);
    }
}
