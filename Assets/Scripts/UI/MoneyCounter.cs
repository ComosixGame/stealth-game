using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyCounter:MonoBehaviour
{
    public float duration;
    public TypeMoney type;
    private int value, startValue = 0;
    private bool isEnd;
    private TextMeshProUGUI text;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        gameManager.OnUpdateMoney.AddListener(OnUpdateMoney);
    }

    private void OnDisable() {
        gameManager.OnUpdateMoney.RemoveListener(OnUpdateMoney);
    }

    public void Counter() {
        float time = duration/value;
        StartCoroutine(StartCount(time));
    }

    private void OnUpdateMoney(int money, int moneyCollected) {
        value = type == TypeMoney.MoneyCollected ? moneyCollected : money;
        Counter();
    }

    IEnumerator StartCount(float time) {
        while(startValue <= value) {
            text.text = startValue.ToString();
            startValue ++;
            yield return new WaitForSeconds(time);
        }
    }
    
}
