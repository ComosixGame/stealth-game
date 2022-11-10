using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using MyCustomAttribute;
#endif

public class TimeBomb : MonoBehaviour
{
#if UNITY_EDITOR
    [Label("Time(s)")]
#endif
    public float time;
    public TextMeshProUGUI text;
    public ParticleSystem explodeEffect;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    private float minutes, seconds, milliseconds, radiusExplode = 12;
    private bool isStartGame, exploded, endGame;
    private GameManager gameManager;
    private SoundManager soundManager;
    public UnityEvent OnAlertTimer;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnStart.AddListener(OnStartGame);
        gameManager.OnEndGame.AddListener(OnEndGame);
    }

    // Update is called once per frame
    void Update()
    {
        if(isStartGame) {
            CountDownTime();
        }
    }

    private void OnDisable() {
        gameManager.OnStart.RemoveListener(OnStartGame);
    }

    private void OnStartGame() {
        isStartGame = true;
    }

    private void CountDownTime() {
        if(time > 0) {
            time -= Time.deltaTime;
        } else {
            time = 0;
        }
        minutes = Mathf.FloorToInt(time / 60);
        seconds = Mathf.FloorToInt(time % 60);
        milliseconds = Mathf.FloorToInt(time % 1 * 1000);
        if(seconds < 10) {
            text.color = Color.red;
        }

        if(seconds < 5) {
            OnAlertTimer?.Invoke();
        }

        if(time <= 0 && !exploded) {
            Explode();
            exploded = true;
        }

        text.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void Explode() {
        soundManager.PlayOneShot(audioClip, volumeScale);
        Instantiate(explodeEffect, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusExplode);
        foreach(Collider collider in colliders) {
            Transform trans = collider.transform;
            Damageable damageable = trans.GetComponentInParent<Damageable>();
            if(damageable != null) {
                Vector3 dir = trans.position - transform.position;
                dir.y = 1;
                Vector3 targetPos = collider.ClosestPoint(trans.position) + dir.normalized * 1f;
                float dis = Vector3.Distance(transform.position, trans.position);
                float f = radiusExplode / dis * 20;
                damageable.TakeDamge(targetPos, dir.normalized * f, 99999);
            }
        }
        Invoke("EndGame", 2f);
    }

    private void OnEndGame(bool isWin) {
        endGame = true;
    }

    private void EndGame() {
        if(!endGame) {
            gameManager.EndGame(false);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusExplode);
    }
#endif
}
