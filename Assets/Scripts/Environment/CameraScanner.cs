using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraScanner : MonoBehaviour
{
    public Transform rootScanner, Camera;
    public float range, speed, alertTime;
    public Vector3[] listPatrol;
    [HideInInspector] public int indexSelectUp, indexSelectForward;
    [SerializeField] private Scanner scanner = new Scanner();
    private int patrolIndex = 0;
    [SerializeField] private bool detected;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleWhenDetected);
        scanner.OnDetectedSubTarget.AddListener(HandleWhenDetectedSubTarget);
        scanner.OnNotDetectedTarget.AddListener(NotDetect);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        scanner.CreataFieldOfView(rootScanner, rootScanner.position, 360, range);
    }

    // Update is called once per frame
    void Update()
    {
        Camera.LookAt(rootScanner.position, GetAxisUp());
        AxisForward();
        scanner.Scan();
        if(!detected) {
            Patrol();
        }
    }

    private void Patrol() {
        if(listPatrol.Length > 0) {
            if(Vector3.Distance(rootScanner.position, listPatrol[patrolIndex]) <= 0.1) {
                patrolIndex ++;
                if(patrolIndex >= listPatrol.Length) {
                    patrolIndex = 0;
                }
            }
            rootScanner.position = Vector3.Lerp(rootScanner.position, listPatrol[patrolIndex], speed * Time.deltaTime);
        }
    }

    private void HandleWhenDetected(List<RaycastHit> listHits) {
        Transform hitTransform = scanner.DetectSingleTarget(listHits);
        if(!detected) {
            gameManager.EnemyTriggerAlert(hitTransform.position, alertTime);
        }
        detected = true;
    }

    private void HandleWhenDetectedSubTarget(Transform subTarget) {
        bool isDetected = subTarget.GetComponentInParent<DeadBody>().isDetected;
        if(!isDetected) {
            gameManager.EnemyTriggerAlert(subTarget.position, alertTime);
            subTarget.GetComponentInParent<DeadBody>().isDetected = true;
        }
    }
    
    private void NotDetect() {
        detected = false;
    }

    private Vector3 GetAxisUp() {
        switch(indexSelectUp) {
            case 0:
                return Vector3.up;
            case 1:
                return Vector3.down;
            case 2:
                return Vector3.right;
            case 3:
                return Vector3.left;
            case 4:
                return Vector3.forward;
            default:
                return Vector3.back;
        }
    }

    private void AxisForward() {
        switch(indexSelectForward) {
            case 0:
                Camera.RotateAround(Camera.position, Camera.right, 90);
                break;
            case 1:
                Camera.RotateAround(Camera.position, -Camera.right, 90);
                break;
            case 2:
                Camera.RotateAround(Camera.position, -Camera.up, 90);
                break;
            case 3:
                Camera.RotateAround(Camera.position, Camera.up, 90);
                break;
            case 4:
                break;
            default:
                Camera.RotateAround(Camera.position, Camera.right, 180);
                break;
        }
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleWhenDetected);
        scanner.OnDetectedSubTarget.RemoveListener(HandleWhenDetectedSubTarget);
        scanner.OnNotDetectedTarget.RemoveListener(NotDetect);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if(rootScanner != null) {
            scanner.EditorGizmo(rootScanner, 360, range);
        }
    }

    [CustomEditor(typeof(CameraScanner))]
    public class EditorCameraScanner : Editor {
        private void OnSceneGUI() {
            Vector3 nextPos;
            CameraScanner cam = target as CameraScanner;
            for(int i = 0; i < cam.listPatrol.Length; i++) {
                cam.listPatrol[i].y = cam.rootScanner.position.y;
                Vector3 patrolPos = cam.listPatrol[i];
                Handles.Label(patrolPos, $"Point {i+1}", "TextField");
                EditorGUI.BeginChangeCheck();
                Vector3 pos = Handles.PositionHandle(patrolPos, Quaternion.identity);
                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(cam,"update patrol position");
                    cam.listPatrol[i] = pos;
                    EditorUtility.SetDirty(cam);
                }

                if(i == cam.listPatrol.Length - 1) {
                    nextPos = cam.listPatrol[0];
                } else {
                    nextPos = cam.listPatrol[i+1];
                }

                Vector3 dirToNextPos = (nextPos - patrolPos).normalized;
                Quaternion rot = Quaternion.LookRotation(dirToNextPos);
                float distanceToNextPoint = Vector3.Distance(patrolPos, nextPos);
                Handles.color = Color.yellow;
                for(int j = 0; j <= distanceToNextPoint/4; j += 2) {
                    Vector3 posOfArrow = pos + dirToNextPos * j;
                    Handles.ArrowHandleCap(i, posOfArrow, rot, 2f, EventType.Repaint);
                }
                Handles.color = Color.blue;
                Handles.DrawDottedLine(patrolPos, nextPos, 3f);
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            CameraScanner cam = target as CameraScanner;
            string[] options = new string[6] {"Y","-Y","X","-X","Z","-Z"};
            EditorGUI.BeginChangeCheck();
            int indexUp = EditorGUILayout.Popup("Axis Up", cam.indexSelectUp, options);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(cam, "Update axis up");
                cam.indexSelectUp = indexUp;
                EditorUtility.SetDirty(cam);

            }
            EditorGUI.BeginChangeCheck();
            int indexForward = EditorGUILayout.Popup("Axis Forward", cam.indexSelectForward, options);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(cam, "Update axis forward");
                cam.indexSelectForward = indexForward;
                EditorUtility.SetDirty(cam);
            }
        }
    }
#endif
}
