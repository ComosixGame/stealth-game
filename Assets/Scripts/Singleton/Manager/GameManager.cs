using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private float healthPlayer;
    private PlayerData playerData;
    private bool isWin, isEnd;
    private Coroutine coroutine;
    public UnityEvent<float> OnUpdateHealthPlayer =  new UnityEvent<float>();
    public UnityEvent<int> OnUpdateMoney =  new UnityEvent<int>();
    public UnityEvent<Vector3> OnEnemyAlert =  new UnityEvent<Vector3>();
    public UnityEvent OnEnemyAlertOff =  new UnityEvent();
    public UnityEvent OnStart =  new UnityEvent();
    public UnityEvent OnPause =  new UnityEvent();
    public UnityEvent OnResume =  new UnityEvent();
    public UnityEvent<int> OnSelectItem =  new UnityEvent<int>();
    public UnityEvent<int> OnBuyItem =  new UnityEvent<int>();
    public UnityEvent<bool> OnEndGame = new UnityEvent<bool>();
    // Start is called before the first frame update
    
    void Start()
    {
        InitGame();
    }
    
    public void UpdatePlayerHealth(float hp) {
        healthPlayer = hp;
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }

    public void UpdateCurrency(int point) {
        if(isEnd) return;
        playerData.money += point;
        OnUpdateMoney?.Invoke(playerData.money);
    }

    public void EnemyTriggerAlert(Vector3 pos, float time) {
        if(coroutine != null) {
            StopCoroutine(coroutine);
        }
        OnEnemyAlert?.Invoke(pos);
        coroutine = StartCoroutine(StartAlert(time));
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

    public void UnlockNewLevel(int indexLevel) {
        List<int> list = playerData.levels;
        if(list.IndexOf(indexLevel) == -1) {
            playerData.levels.Add(indexLevel);
            playerData.Save();
        }
    }

    public void InitGame() {
        isEnd = false;
        playerData = PlayerData.Load();
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
        OnUpdateMoney?.Invoke(playerData.money);
    }

    public void EndGame(bool win) {
        isWin = win;
        OnEndGame?.Invoke(isWin);
        OnUpdateMoney?.Invoke(playerData.money);
        playerData.Save();
    }

    public bool BuyItem(int id, int price) {
        if(playerData.money >= price) {
            if(playerData.characters.IndexOf(id) == -1) {
                UpdateCurrency(-price);
                playerData.characters.Add(id);
                playerData.Save();
                OnBuyItem?.Invoke(id);
                return true;
            }
        }

        return false;
    }

    public void SelectItem(int id) {
        playerData.selectedCharacter = id;
        playerData.Save();
        OnSelectItem?.Invoke(id);
    }

    IEnumerator StartAlert(float time) {
        yield return new WaitForSeconds(time);
        OnEnemyAlertOff?.Invoke();
    }

}
