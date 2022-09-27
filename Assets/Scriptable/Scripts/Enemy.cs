using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [Range(0, 360)] public float detectionAngle;
    public float
        viewDistance,
        speed = 3,
        angularSpeed = 120,
        acceleration = 8,
        IdleTime = 2,
        alertTime = 10,
        speedRotation = 7;
}
