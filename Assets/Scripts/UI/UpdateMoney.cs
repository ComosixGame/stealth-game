using UnityEngine;
using TMPro;

public class UpdateMoney : MonoBehaviour
{
    private TextMeshProUGUI textPro;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        textPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        gameManager.OnUpdateMoney.AddListener(UpdateText);
    }

    private void UpdateText(int money) {
        textPro.text = money.ToString();
    }

    private void OnDisable() {
        gameManager.OnUpdateMoney.RemoveListener(UpdateText);
    }
}
