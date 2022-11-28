using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using MyCustomAttribute;
#endif
public class KeyPad : MonoBehaviour, IInteractable
{
    [SerializeField] private Command command;
    public TextMeshProUGUI textOutPut;
    public int length;
    public Canvas keyPadUI;
    [ReadOnly, SerializeField, Label("Code(Auto generation)")]
    private string code;
    private string input;
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
            Invoke("Execute", 1);
        } else {
            textOutPut.text = "Fail";
            input = "";
        }
    }

    private void OnCancel() {
        keyPadUI.gameObject.SetActive(false);
    }

    private void Execute() {
        if(command != null) {
            command.Execute();
        }
        keyPadUI.gameObject.SetActive(false);
    }

    public void Interact()
    {
        keyPadUI.gameObject.SetActive(true);
    }
}
