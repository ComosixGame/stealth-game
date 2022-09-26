using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private float healthPlayer;
    public UnityEvent<float> OnUpdateHealthPlayer =  new UnityEvent<float>();
    // Start is called before the first frame update
    void Start()
    {
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthPlayer(float hp) {
        healthPlayer = hp;
    }

    public void PlayerGetDamage(float damage) {
        healthPlayer -= damage;
        OnUpdateHealthPlayer?.Invoke(healthPlayer);
    }
}
