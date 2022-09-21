using UnityEngine;
using UnityEditor;


//custom editor for enemy patrol
[CustomEditor(typeof(EnemyBehaviourScript))]
public class HandlePosition : Editor {
    private void OnSceneGUI() {
        var t = target as EnemyBehaviourScript;
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
            Handles.DrawDottedLines(listPoint, segmentIndices, 2);

            // Draw arrow dir
            int nextIndexPoint = i >= listPoint.Length - 1 ? 0 : i + 1;
            float distanceToNextPoint = Vector3.Distance(pos, listPoint[nextIndexPoint]);
            for(int j = 0; j <= distanceToNextPoint/4; j += 2) {
                Vector3 dir = (listPoint[nextIndexPoint] - pos).normalized;
                if(dir != Vector3.zero) {
                    Vector3 posOfArrow = pos + dir * j;
                    Handles.ArrowHandleCap(i,posOfArrow ,Quaternion.LookRotation(dir), 2.5f, EventType.Repaint);
                }
            }

            //Draw a button point
            Handles.Label(pos, $"Point {i+1}","button");

            //begin check change on editor
            EditorGUI.BeginChangeCheck();
            
            Vector3 newPos = Handles.PositionHandle(pos, new Quaternion(0,0,0,1));
            if(EditorGUI.EndChangeCheck()) {
                // update position point
                Undo.RecordObject(t, "Update Patrol point");
                listPoint[i] = newPos;
            }
        }
    }
}
