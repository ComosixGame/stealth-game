#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//custom editor for enemy patrol
[CustomEditor(typeof(EnemyBehaviourScript))]
[CanEditMultipleObjects]
public class EditorEnemyBehaviour : Editor {
    bool showToggle;
    private void OnSceneGUI() {
        EnemyBehaviourScript t = target as EnemyBehaviourScript;
        if(t.patrolList != null) {
            CustomPatrolPoint(t);
            if(t.typePatrol == EnemyBehaviourScript.TypePatrol.StandInPlace) {
                CustomStandPoint(t);
            }
        }
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EnemyBehaviourScript enemyBehaviour = target as EnemyBehaviourScript;
        if(enemyBehaviour.typePatrol == EnemyBehaviourScript.TypePatrol.StandInPlace) {
            EditorGUI.BeginChangeCheck();
            Vector3 standPos = EditorGUILayout.Vector3Field("Stand Position",enemyBehaviour.standPos);
            
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(enemyBehaviour, "Update stand point");
                enemyBehaviour.standPos = standPos;
            }
        }
    }

    private void CustomPatrolPoint(EnemyBehaviourScript t) {
        Vector3[] listPoint = t.patrolList;

        // for each line segment we need two indices into the points array:
        // the index to the start and the end point
        int[] segmentIndices = new int[listPoint.Length * 2];

        // create the points and line segments indices
        int prevIndex = listPoint.Length - 1;
        int pointIndex = 0;
        int segmentIndex = 0;

        for(int i = 0; i< t.patrolList.Length; i++) {
            Vector3 pos = listPoint[i];

            // the index to the start of the line segment
            segmentIndices[segmentIndex] = prevIndex;
            segmentIndex++;

            // the index to the end of the line segment
            segmentIndices[segmentIndex] = pointIndex;
            segmentIndex++;

            pointIndex++;
            prevIndex = i;
                
            //Draw a list of indexed dotted line segments
            Handles.color = Color.blue;

            if(t.typePatrol == EnemyBehaviourScript.TypePatrol.MoveAround) {
                // Draw arrow dir if type patrol is move around
                Handles.DrawDottedLines(listPoint, segmentIndices, 2);
                int nextIndexPoint = i >= listPoint.Length - 1 ? 0 : i + 1;
                float distanceToNextPoint = Vector3.Distance(pos, listPoint[nextIndexPoint]);
                for(int j = 0; j <= distanceToNextPoint/4; j += 2) {
                    Vector3 dir = (listPoint[nextIndexPoint] - pos).normalized;
                    if(dir != Vector3.zero) {
                        Vector3 posOfArrow = pos + dir * j;
                        Handles.ArrowHandleCap(i,posOfArrow ,Quaternion.LookRotation(dir), 2.5f, EventType.Repaint);
                    }
                }
            } else {
                // Draw arrow dir if type patrol is stand in place
                Handles.color = Color.blue;
                Quaternion rot = Quaternion.LookRotation((pos - t.transform.position).normalized);
                Handles.ArrowHandleCap(i, t.transform.position, rot, 5f, EventType.Repaint);
            }

            //Draw a button point
            Handles.Label(pos, $"Point {i+1}","TextField");

            //begin check change on editor
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(pos, Quaternion.identity);

            if(EditorGUI.EndChangeCheck()) {
                // update position point
                Undo.RecordObject(t, "Update Patrol point");
                listPoint[i] = newPos;
            }
        }
    }

    private void CustomStandPoint(EnemyBehaviourScript t) {
        Handles.Label(t.standPos,"Stand Pos","TextField");
        Handles.DrawDottedLine(t.standPos, t.transform.position,2);
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(t.standPos, Quaternion.identity);
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(t, "Update stand point");
            t.standPos = newPos;
        }
    }
}

#endif
