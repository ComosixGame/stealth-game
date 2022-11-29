using UnityEngine;
using UnityEngine.Events;
using TMPro;
#if UNITY_EDITOR
using MyCustomAttribute;
using UnityEditor;
#endif
public class KeyPad : MonoBehaviour
{
    public LayerMask layer;
    [SerializeField] private Command command;
    public TextMeshProUGUI textOutPut;
    public TextMeshPro hintText;
    public int length;
    public AudioClip audioClipSuccess, audioClipFail;
    [Range(0,1)] public float volumeScale;
    public Canvas keyPadUI;
    [ReadOnly, SerializeField, Label("Code(Auto generation)")]
    private string code;
    private string input;
    private SoundManager soundManager;
    public UnityEvent OnSuccess;
    private bool success;

    private void Awake() {
        soundManager = SoundManager.Instance;
    }

    private void OnEnable() {
        KeyPadBtn.OnEnteringPass += OnEnteringPass;
        KeyPadBtn.OnDel += OnDel;
        KeyPadBtn.OnEnter += OnEnter;
        KeyPadBtn.OnCancel += OnCancel;
    }
    private void Start() {
        for(int i = 0; i<length; i++) {
            code += Random.Range(0,9);
        }
        hintText.text = code;
    }

    private void OnTriggerEnter(Collider other) {
        if(success) return;
        if((layer & (1<< other.gameObject.layer))!= 0) {
            keyPadUI.gameObject.SetActive(true);
        }
    }

    private void OnDisable() {
        KeyPadBtn.OnEnteringPass -= OnEnteringPass;
        KeyPadBtn.OnDel -= OnDel;
        KeyPadBtn.OnEnter -= OnEnter;
        KeyPadBtn.OnCancel -= OnCancel;
    }

    private void OnEnteringPass(string key) {
        input += key;
        textOutPut.text = input;
    }

    private void OnDel() {
        input = input.Remove(textOutPut.text.Length - 1);
        textOutPut.text = input;
    }

    private void OnEnter() {
        if(input == code) {
            textOutPut.text = "Success";
            soundManager.PlayOneShot(audioClipSuccess, volumeScale);
            Invoke("Execute", 1);
        } else {
            textOutPut.text = "Fail";
            input = "";
            soundManager.PlayOneShot(audioClipFail, volumeScale);
        }
    }

    private void OnCancel() {
        keyPadUI.gameObject.SetActive(false);
        input = "";
        textOutPut.text = "Enter Code";
    }

    private void Execute() {
        if(command != null) {
            command.Execute();
        }
        keyPadUI.gameObject.SetActive(false);
        success = true;
        OnSuccess?.Invoke();
    }

    public void Interact()
    {
        keyPadUI.gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Handles.Label(transform.position, "Commander","TextField");
        if(command != null) {
            Gizmos.DrawLine(command.transform.position, transform.position);
            Handles.Label(command.transform.position, "Interact Object","TextField");
        }
    }

    private void OnDrawGizmos() {
        Handles.color = Color.blue;
        if(command != null) {
            Handles.DrawDottedLine(transform.position, command.transform.position, 3f);
        }
    }
#endif
}
