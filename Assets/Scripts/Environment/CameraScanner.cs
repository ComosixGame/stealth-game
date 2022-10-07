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
    [HideInInspector] public int indexSelect;
    [SerializeField] private Scanner scanner = new Scanner();
    private int patrolIndex = 0;
    private bool detected;
    private Vector3 axisUp;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        scanner.OnDetectedTarget.AddListener(HandleWhenDetected);
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
        Camera.LookAt(rootScanner, axisUp);
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
    
    private void NotDetect() {
        detected = false;
    }

    private void OnDisable() {
        scanner.OnDetectedTarget.RemoveListener(HandleWhenDetected);
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
            int index = EditorGUILayout.Popup("Axis Up", cam.indexSelect, options);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(cam, "Update axis up");
                cam.indexSelect = index; 
                switch(cam.indexSelect) {
                    case 1:
                        cam.axisUp = Vector3.down;
                        break;
                    case 2:
                        cam.axisUp = Vector3.right;
                        break;
                    case 3:
                        cam.axisUp = Vector3.left;
                        break;
                    case 4:
                        cam.axisUp = Vector3.forward;
                        break;
                    case 5:
                        cam.axisUp = Vector3.back;
                        break;
                    default:
                        cam.axisUp = Vector3.up;
                        break;
                }
            }
        }
    }
#endif
}
