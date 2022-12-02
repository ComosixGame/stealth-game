using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
#if UNITY_EDITOR
    [Label("Bomb Disposal Time(s)")]
#endif
    public float bombDefuseTime;
    public float radiusExplode = 12;
    public GameObject TimeBombUI;
    public TextMeshProUGUI text;
    public Slider progressBar;
    public ParticleSystem explodeEffect;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    public LayerMask layer;
    public VolumetricLines.VolumetricLineBehavior volumetricLine;
    private float minutes, seconds, milliseconds, timerBlinkLed;
    private bool isStartGame, explode, endGame, defuse, isAlert, win;
    private Camera cam;
    private float volumetricLineLineWidth = 2f, timeBlinkLed = 0.5f;
    private AudioSource audioSource;
    private GameManager gameManager;
    private SoundManager soundManager;
    public UnityEvent OnAlertTimer, OnExplode, OnExploded, OnDefuse;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        audioSource = GetComponent<AudioSource>();
        TimeBombUI.SetActive(true);
    }

    private void OnEnable() {
        gameManager.OnStart.AddListener(OnStartGame);
        gameManager.OnEndGame.AddListener(OnEndGame);
        soundManager.OnMute.AddListener(OnMuteGame);
    }

    private void Start() {
        cam = Camera.main;
        progressBar.maxValue = bombDefuseTime;
        audioSource.mute =  gameManager.settingData.mute;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStartGame && !defuse) {
            CountDownTime();
        }

        if(!defuse) {
            if(Time.time >= timerBlinkLed) {
                volumetricLine.LineWidth += volumetricLineLineWidth;
                volumetricLineLineWidth *= -1;
                timerBlinkLed = Time.time + timeBlinkLed;
            }
        } else {
            volumetricLineLineWidth = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(explode) return;
        if((layer & (1 << other.gameObject.layer)) != 0 && !defuse) {
            progressBar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(explode) return;
        if((layer & (1 << other.gameObject.layer)) != 0 && !defuse) {
            progressBar.transform.position = cam.WorldToScreenPoint(transform.position);
            if(progressBar.value >= progressBar.maxValue) {
                defuse = true;
                progressBar.gameObject.SetActive(false);
                OnDefuse?.Invoke();
                win = true;
                Invoke("EndGame", 0.5f);
                return;
            }
            progressBar.value += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other) {
        if((layer & (1 << other.gameObject.layer)) != 0) {
            progressBar.gameObject.SetActive(false);
            progressBar.value = progressBar.minValue;
        }
    }

    private void OnDisable() {
        gameManager.OnStart.RemoveListener(OnStartGame);
        gameManager.OnEndGame.RemoveListener(OnEndGame);
        soundManager.OnMute.RemoveListener(OnMuteGame);
    }

    private void OnStartGame(bool bossFight) {
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
        if(time < 10) {
            text.color = Color.red;
        }

        if(time < 5 && !isAlert) {
            OnAlertTimer?.Invoke();
            isAlert = true;
        }

        if(time <= 0 && !explode) {
            progressBar.gameObject.SetActive(false);
            explode = true;
            OnExplode?.Invoke();
            Invoke("Explode", 2);
        }

        text.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void Explode() {
        OnExploded?.Invoke();
        soundManager.PlayOneShot(audioClip, volumeScale);
        Instantiate(explodeEffect, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusExplode);
        foreach(Collider collider in colliders) {
            Transform trans = collider.transform;
            IDamageable damageable = trans.GetComponentInParent<IDamageable>();
            if(damageable != null) {
                Vector3 dir = trans.position - transform.position;
                dir.y = 1;
                Vector3 targetPos = collider.ClosestPoint(trans.position) + dir.normalized * 1f;
                float dis = Vector3.Distance(transform.position, trans.position);
                float f = radiusExplode / dis * 30;
                damageable.TakeDamge(targetPos, dir.normalized * f, 99999);
            }
        }
        win = false;
        Invoke("EndGame", 2f);
    }

    private void OnEndGame(bool isWin) {
        endGame = true;
    }

    private void EndGame() {
        if(!endGame) {
            gameManager.EndGame(win);
        }
    }

    private void OnMuteGame(bool mute) {
        audioSource.mute = mute;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusExplode);
    }
#endif
}
