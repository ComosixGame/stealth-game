using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private float healthPlayer;
    [SerializeField] private int money;
    public UnityEvent<float> OnUpdateHealthPlayer =  new UnityEvent<float>();
    public UnityEvent<int> OnUpdateMoney =  new UnityEvent<int>();
    public UnityEvent<Vector3> OnEnemyAlert =  new UnityEvent<Vector3>();
    public UnityEvent OnEnemyAlertOff =  new UnityEvent();
    public UnityEvent OnPause =  new UnityEvent();
    public UnityEvent OnResume =  new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
        OnUpdateMoney?.Invoke(money);
    }
    
    public void UpdatePlayerHealth(float hp) {
        healthPlayer = hp;
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }

    public void UpdateCurrency(int point) {
        money += point;
        OnUpdateMoney?.Invoke(money);
    }

    public void EnemyTriggerAlert(Vector3 pos, float time) {
        StopCoroutine("StartAlert");
        OnEnemyAlert?.Invoke(pos);
        StartCoroutine(StartAlert(time));
    }

    public void PauseGame() {
        OnPause?.Invoke();
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        OnResume?.Invoke();
        Time.timeScale = 1;
    }

    IEnumerator StartAlert(float time) {
        yield return new WaitForSeconds(time);
        OnEnemyAlertOff?.Invoke();
    }

}
