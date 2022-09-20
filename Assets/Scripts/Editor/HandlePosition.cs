using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyBehaviourScript))]
public class HandlePosition : Editor {
    private void OnSceneGUI() {
        var t = target as EnemyBehaviourScript;
        for(var i = 0; i< t.patrolList.Length; i++) {
            Vector3 pos = t.patrolList[i];
            Handles.Label(pos, $"Point {i+1}","button");
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(pos, new Quaternion(0,0,0,1));
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(t, "Update Patrol point");
                t.patrolList[i] = newPos;
            }
        }
    }
}
