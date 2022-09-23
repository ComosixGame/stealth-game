using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private float heathPlayer;
    public UnityEvent<float> OnUpdateHeathPlayer = new UnityEvent<float>();
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerGetDamage(float damage) {
        heathPlayer -= damage;
        OnUpdateHeathPlayer?.Invoke(heathPlayer);
    }
}
