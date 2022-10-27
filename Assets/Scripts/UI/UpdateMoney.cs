using UnityEngine;
using TMPro;

public class UpdateMoney : MonoBehaviour
{
    public enum TypeMoney {
        PlayerMoney,
        MoneyCollected
    }
    public TypeMoney type;
    private TextMeshProUGUI textPro;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        textPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        gameManager.OnUpdateMoney.AddListener(UpdateText);
    }

    private void UpdateText(int money, int moneyCollected) {
        if(type == TypeMoney.PlayerMoney) {
            textPro.text = money.ToString();
        } else {
            textPro.text = moneyCollected.ToString();
        }
    }

    private void OnDisable() {
        gameManager.OnUpdateMoney.RemoveListener(UpdateText);
    }
}
