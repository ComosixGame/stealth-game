using UnityEngine;

public abstract class AttackAction : MonoBehaviour {
    [SerializeField] private float damage;
    [SerializeField] private AudioClip audioEffect;
    public abstract void Attack(Transform _transform);
    
}