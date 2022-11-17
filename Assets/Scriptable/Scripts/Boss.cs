using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Enemy/Boss")]
public class Boss : ScriptableObject
{
    public string nameBoss;
    public int numberOfBullets;
    public float timeAttack, timeReadyAttack, delayAttack, speedBullet, damage;
    public AudioClip audioClip;
    [Range(1,360)] public float angleAttack;
    public int coinBonus;
    public float health;
}