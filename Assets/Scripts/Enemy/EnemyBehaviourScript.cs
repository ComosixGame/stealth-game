using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{
    public Vector3[] patrolList;
    [SerializeField] private PlayerScanner playerScanner = new PlayerScanner();
    [SerializeField] private Enemy enemy;
    private NavMeshAgent agent;
    private GameObject FieldOfView;
    private int patrolIndex = 0;

    private enum State {
        PATROL,
        CHASE,
        ATTACK,
    }

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
        Patrol();
        playerScanner.Scan(transform);
    }

    private void Patrol() {
        Vector3 walkPoint = patrolList[patrolIndex];

        if(agent.remainingDistance <= agent.stoppingDistance) {
            patrolIndex++;
            if(patrolIndex >= patrolList.Length) {
                patrolIndex = 0;
            }

            agent.SetDestination(walkPoint);
        }
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(transform, enemy.detectionAngle, enemy.viewDistance);
    }
#endif
}
