using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private float healthPlayer;
    private int money;
    private bool isWin;
    public UnityEvent<float> OnUpdateHealthPlayer =  new UnityEvent<float>();
    public UnityEvent<int> OnUpdateMoney =  new UnityEvent<int>();
    public UnityEvent<Vector3> OnEnemyAlert =  new UnityEvent<Vector3>();
    public UnityEvent OnEnemyAlertOff =  new UnityEvent();
    public UnityEvent OnStart =  new UnityEvent();
    public UnityEvent OnPause =  new UnityEvent();
    public UnityEvent OnResume =  new UnityEvent();
    public UnityEvent<bool, int> OnEndGame = new UnityEvent<bool, int>();
    // Start is called before the first frame update
    void Start()
    {
        ResetGame();
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

    public void StartGame() {
        OnStart?.Invoke();
    }
    public void PauseGame() {
        OnPause?.Invoke();
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        OnResume?.Invoke();
        Time.timeScale = 1;
    }

    public void ResetGame() {
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
        OnUpdateMoney?.Invoke(money);
    }

    public void EndGame(bool win) {
        isWin = win;
        OnEndGame?.Invoke(isWin, money);
        OnUpdateMoney?.Invoke(money);
    }

    IEnumerator StartAlert(float time) {
        yield return new WaitForSeconds(time);
        OnEnemyAlertOff?.Invoke();
    }

}
