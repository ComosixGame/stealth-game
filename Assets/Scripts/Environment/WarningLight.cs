using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLight : MonoBehaviour
{
    public VolumetricLines.VolumetricLineBehavior volumetricLine;
    private bool alert;
    private float timerBlinkLed;
    private int i = 1;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnEnemyAlert.AddListener(AlertON);
        gameManager.OnEnemyAlertOff.AddListener(AlertOFF);
    }

    private void Update() {
        if(alert) {
            volumetricLine.LineWidth += i * 30f * Time.deltaTime;
            if(volumetricLine.LineWidth >= 20) {
                i = -1;
            } else if( volumetricLine.LineWidth <= 0) {
                i = 1;
            }
        }
    }

    private void AlertON(Vector3 pos) {
        alert = true;
    }

    private void AlertOFF() {
        alert = false;
        volumetricLine.LineWidth = 0;
    }

    private void OnDisable() {
        gameManager.OnEnemyAlert.RemoveListener(AlertON);
        gameManager.OnEnemyAlertOff.RemoveListener(AlertOFF);
    }
}
