using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyPadBtn : MonoBehaviour
{
    public string key;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    private Button button;
    private SoundManager soundManager;
    public static event Action<string> OnEnteringPass;
    public static event Action OnEnter;
    public static event Action OnDel;
    public static event Action OnCancel;
    private void Awake() {
        button = GetComponent<Button>();
        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        soundManager.PlayOneShot(audioClip, volumeScale);
        switch(key) {
            case "del":
                OnDel?.Invoke();
                break;
            case "enter":
                OnEnter?.Invoke();
                break;
            case "cancel":
                OnCancel?.Invoke();
                break;
            default:
                OnEnteringPass?.Invoke(key);
                break;
        }
    }

    private void OnDisable() {
        button.onClick.RemoveListener(OnClick);
    }
}
