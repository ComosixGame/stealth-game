using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{
    public Vector3[] patrolList;
    [SerializeField] private PlayerScanner playerScanner = new PlayerScanner();
    [SerializeField] private Enemy enemy;
    private NavMeshAgent agent;
    private GameObject FieldOfView;
    private int patrolIndex = 0;
    private Vector3 LastplayerPosition;

    private enum EnemyAction {
        PATROL,
        CHASE,
        ATTACK,
        LOOKING,
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        playerScanner.OnDetectedTarget.AddListener(Attack);
        playerScanner.OnLostTarget.AddListener(Patrol);
    }

    // Start is called before the first frame update
    void Start()
    {
        //creata field of view
        FieldOfView = playerScanner.CreataFieldOfView(transform, transform.position);

        playerScanner.SetFovAngel(enemy.detectionAngle);
        playerScanner.SetViewDistence(enemy.viewDistance);
    }

    // Update is called once per frame
    void Update()
    {
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

    private void Chase() {
        agent.SetDestination(LastplayerPosition);
    }

    private void Attack(Transform playerTransform) {
        agent.SetDestination(transform.position);
        Vector3 dirLook = playerTransform.position - transform.position;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        if(Quaternion.Angle(transform.rotation, rotLook) > 10) {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, 2f * Time.deltaTime);
        } else {
            Debug.Log("bùm chíu");
            transform.rotation =  rotLook;
        }

    }


    private void OnDisable() {
   
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(transform, enemy.detectionAngle, enemy.viewDistance);
    }
#endif
}
