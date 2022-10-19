using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Character/Enemy")]
public class Enemy : ScriptableObject
{
    [Range(0, 360)] public float detectionAngle;
    public float
        health = 1,
        moneyBonus = 1,
        viewDistance,
        speed = 3,
        speedPatrol = 5,
        angularSpeed = 120,
        acceleration = 8,
        IdleTime = 2,
        alertTime = 10,
        speedRotation = 7;
}
