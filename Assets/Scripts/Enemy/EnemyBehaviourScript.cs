using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{
    [SerializeField] private PlayerScanner playerScanner = new PlayerScanner();
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform[] patrolList;
    private NavMeshAgent agent;
    private GameObject FieldOfView;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();

        //creata field of view
        FieldOfView = playerScanner.CreataFieldOfView(transform, transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScanner.SetFovAngel(enemy.detectionAngle);
        playerScanner.SetViewDistence(enemy.viewDistance);
    }

    // Update is called once per frame
    void Update()
    {
        playerScanner.renderFieldOfView(transform);
    }

    private void Patrol() {
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(transform, enemy.detectionAngle, enemy.viewDistance);
    }

#endif
}
