#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerAttack))]
public class EditorPlayerAttack : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        PlayerAttack PlayerAttack = target as PlayerAttack;
        SphereCollider collider = PlayerAttack.GetComponent<SphereCollider>();
        collider.radius = PlayerAttack.range;
    }
}

#endif
