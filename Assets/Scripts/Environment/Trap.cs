using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public float volumeScale;
    private bool ready, PowerOff, turnOn = true;
    private float timeNextAttack, nextSwitch;
    private AudioSource audioSource;
    private SoundManager soundManager;

    private void Awake() {
        soundManager = SoundManager.Instance;
        audioSource = soundManager.AddAudioSource(gameObject);
        audioSource.clip = audioClip;
        audioSource.volume = volumeScale;
        audioSource.loop = true;

    }


    private void Update() {
        if(typemode == Typemode.Blink && !PowerOff) {
            if(Time.time > nextSwitch){
                turnOn = !turnOn;
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

            if(nextSwitch - Time.time <= warningBeforeOn && !turnOn) {
                if(warningEffect.isStopped) {
                    warningEffect.Play();
                }
            } else {
                if(warningEffect.isPlaying) {
                    warningEffect.Stop();
                }
            }

        }
    }

    private void OnTriggerStay(Collider other) {
        if(PowerOff || !turnOn) {
            audioSource.Stop();
            return;
        }
        if(!audioSource.isPlaying) {
            audioSource.Play();
        }
        
        if((layer & (1 << other.gameObject.layer)) != 0) {
            Vector3  dir = other.transform.position - transform.position;
            dir.y = 0;

            if(Time.time >= timeNextAttack) {
                ready = true;
            }

            if(ready) {
                other.GetComponent<Damageable>().TakeDamge(other.transform.position, dir * 20, damage);
                timeNextAttack = Time.time + delayTimeAttack;
                ready = false;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        ready = true;
        timeNextAttack = 0;
    }

    private void OnTriggerExit(Collider other) {
        audioSource.Stop();
    }
    
    public override void Execute()
    {
        PowerOff = true;
        effect.SetActive(false);
        if(typemode == Typemode.Blink) {
            warningEffect.Stop();
        }
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
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
            }
        }
    }
#endif
    
}
