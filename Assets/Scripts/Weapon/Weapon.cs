using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float force;
    public AudioClip audioEffect;
    [SerializeField] protected float damage, delayAttack;
    protected float timeNextAttack;
    public abstract void Attack(Transform target, LayerMask layerMask);
}
