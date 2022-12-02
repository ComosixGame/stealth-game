using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Trap : Command
{
    public enum Typemode {
        Normal,
        Blink,
    }

    public float damage;
    [HideInInspector] public float delayTimeAttack;
    [HideInInspector] public float onDuration, offDuration, warningBeforeOn;
    public LayerMask layer;
    public GameObject effect;
    [HideInInspector] public ParticleSystem warningEffect;
    public Typemode typemode;
    public AudioClip audioClip;
    [HideInInspector] public bool turnOn = true;
    [HideInInspector] public float  phaseDifference;
    [Range(0,1)] public float volumeScale;
    private bool ready, PowerOff;
    private float timeNextAttack, nextSwitch;
    private AudioSource audioSource;
    private SoundManager soundManager;
    [SerializeField] private UnityEvent OnExecute;

    private void Awake() {
        soundManager = SoundManager.Instance;
        audioSource = soundManager.AddAudioSource(gameObject);
        audioSource.clip = audioClip;
        audioSource.volume = volumeScale;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void OnEnable() {
        soundManager.OnMute.AddListener(OnMuteGame);
    }

    private void Start() {
        if(typemode != Typemode.Blink) return;
        effect.SetActive(turnOn);
        nextSwitch = turnOn ? Time.time + onDuration:Time.time + offDuration;
        nextSwitch -= !turnOn? phaseDifference : 0;
    }

    private void Update() {
        if(typemode == Typemode.Blink && !PowerOff) {
            if(Time.time > nextSwitch){
                turnOn = !turnOn;
                nextSwitch = Time.time;
                nextSwitch += (turnOn ? onDuration : offDuration);
                if(turnOn) {
                    if(!effect.activeSelf) {
                        effect.SetActive(true);
                    }
                } else {
                    if(effect.activeSelf) {
                        effect.SetActive(false);
                    }
                }
            }

            ParticleSystem.MainModule warningEffectMain =  warningEffect.main;
            float timeBeforeTurnOn = nextSwitch - Time.time;

            if(!turnOn) {
                if(timeBeforeTurnOn <= warningBeforeOn) {
                    warningEffectMain.startSizeY = 1.5f;
                    if(!warningEffect.gameObject.activeSelf) {
                        warningEffect.gameObject.SetActive(true);
                    }
                } else {
                    if(warningEffect.gameObject.activeSelf) {
                        warningEffect.gameObject.SetActive(false);
                    }
                }
            } else {
                float startSizeY = warningEffectMain.startSizeY.constant;
                warningEffectMain.startSizeY = Mathf.Lerp(startSizeY, 0.3f, 2f * Time.deltaTime);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(PowerOff || !turnOn) {
            audioSource.Stop();
            return;
        }
        
        if((layer & (1 << other.gameObject.layer)) != 0) {
            Transform trans = other.transform;
            if(!audioSource.isPlaying) {
                audioSource.Play();
            }
            Vector3  dir = trans.position - transform.position;
            dir.y = 0;

            if(Time.time >= timeNextAttack) {
                ready = true;
            }

            if(ready) {
                Collider collider = other.GetComponent<Collider>();
                Vector3 hitPos = collider.ClosestPoint(trans.position) + trans.up * 2f - dir.normalized * 0.3f;
                other.GetComponent<IDamageable>().TakeDamge(hitPos, dir.normalized * 50, damage);
                timeNextAttack = Time.time + delayTimeAttack;
                ready = false;
            }
        }
    }
    
    private void OnTriggerExit(Collider other) {
        ready = true;
        timeNextAttack = 0;
        audioSource.Stop();
    }

    private void OnMuteGame(bool isMute) {
        audioSource.mute = isMute;
    }
    
    public override void Execute()
    {
        PowerOff = true;
        effect.SetActive(false);
        if(typemode == Typemode.Blink) {
            warningEffect.Stop();
        }
        OnExecute?.Invoke();
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable() {
        soundManager.OnMute.RemoveListener(OnMuteGame);
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(Trap))]
    public class EditorTrap : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Trap trap =  target as Trap;
            EditorGUI.BeginChangeCheck();
            float delayTime = EditorGUILayout.FloatField("Delay Time Attack", trap.delayTimeAttack);
            if(EditorGUI.EndChangeCheck()) {
                UnityEditor.Undo.RecordObject(trap, "Update Delay Time Attack");
                trap.delayTimeAttack = delayTime;
                // EditorUtility.SetDirty();
            }
            if(trap.typemode == Trap.Typemode.Blink) {
                EditorGUI.BeginChangeCheck();
                ParticleSystem particleSystem = EditorGUILayout.ObjectField("Warning Effect",trap.warningEffect, typeof(ParticleSystem), true) as ParticleSystem;
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(trap, "Update Warning Effect");
                    trap.warningEffect =  particleSystem;
                    EditorUtility.SetDirty(trap);
                }

                EditorGUI.BeginChangeCheck();
                float timeTurnOff = EditorGUILayout.FloatField("Off Duration", trap.offDuration);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(trap, "Update Off Duration");
                    trap.offDuration = timeTurnOff;
                }
                EditorGUI.BeginChangeCheck();
                float timeTurnOn = EditorGUILayout.FloatField("On Duration", trap.onDuration);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(trap, "Update On Duration");
                    trap.onDuration = timeTurnOn;
                }

                EditorGUI.BeginChangeCheck();
                float warningBeforeOn = EditorGUILayout.FloatField("Warning Before Turn On", trap.warningBeforeOn);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(trap, "Update warning Before On");
                    trap.warningBeforeOn = warningBeforeOn;
                }


                EditorGUI.BeginChangeCheck();
                bool turnOn = EditorGUILayout.Toggle("Turn On", trap.turnOn);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(trap, "Update turnOn");
                    trap.turnOn = turnOn;
                }
                if(!trap.turnOn) {
                    EditorGUI.BeginChangeCheck();
                    float phaseDifference = EditorGUILayout.FloatField("Độ lệch Pha", trap.phaseDifference);
                    if(EditorGUI.EndChangeCheck()) {
                        UnityEditor.Undo.RecordObject(trap, "Update phaseDifference");
                        trap.phaseDifference = phaseDifference;
                    }
                }
            }
        }
    }
#endif
    
}
