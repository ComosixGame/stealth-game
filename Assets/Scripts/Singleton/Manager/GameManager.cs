using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private float healthPlayer;
    public UnityEvent<float> OnUpdateHealthPlayer =  new UnityEvent<float>();
    public UnityEvent<Vector3> OnEnemyAlert =  new UnityEvent<Vector3>();
    public UnityEvent OnEnemyAlertOff =  new UnityEvent();
    // Start is called before the first frame update
    void Start()
    {
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdatePlayerHealth(float hp) {
        healthPlayer = hp;
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }

    public void EnemyTriggerAlert(Vector3 pos, float time) {
        StopCoroutine("StartAlert");
        OnEnemyAlert?.Invoke(pos);
        StartCoroutine(StartAlert(time));
    }

    IEnumerator StartAlert(float time) {
        yield return new WaitForSeconds(time);
        OnEnemyAlertOff?.Invoke();
    }

}
