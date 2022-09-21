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
    private Vector3 playerPosition;
    private enum State {
        Idle,
        Patrol,
        Chase,
        Attack
    }
    private State state;
    private State prevState;
    private float IdleTime;
    private Vector3 randomDirLook = Vector3.zero;
    private float speedRotation = 3;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();

        playerScanner.OnDetectedTarget.AddListener(transformPlayer=> {
            playerPosition = transformPlayer.position;
            state = State.Attack;
        });

        playerScanner.OnNotDetectedTarget.AddListener(HandelChangeState);
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

        switch(state) {
            case State.Idle:
                prevState = state;
                Idle();
                break;
            case State.Patrol:
                prevState = state;
                IdleTime = 0;
                Patrol();
                break;
            case State.Attack:
                prevState = state;
                IdleTime = 0;
                Attack(playerPosition);
                break;
            case State.Chase:
                prevState = state;
                IdleTime = 0;
                Chase(playerPosition);
                break;
            default:
                break;
        }
    }

    
    private void HandelChangeState() {
        switch(prevState) {
            case State.Idle:
                break;
            case State.Patrol:
                break;
            case State.Attack:
                state = State.Chase;
                break;
            case State.Chase:
                break;
            default:
                break;
        }
    }

    private void Idle() {
        IdleTime += Time.deltaTime;
        agent.SetDestination(transform.position);
        if(IdleTime >= 2) {
            state = State.Patrol;
            IdleTime = 0;
        }
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

    private void Attack(Vector3 playerPos) {
        agent.SetDestination(transform.position);
        Vector3 dirLook = playerPos - transform.position;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, speedRotation * Time.deltaTime);
        if(Quaternion.Angle(transform.rotation, rotLook) <= 10) {
            Debug.Log("bùm chíu");
        }
    }

    private void Chase(Vector3 playerPos) {
        agent.SetDestination(playerPos);
        if(Vector3.Distance(transform.position, playerPos) <= 2f) {
            state = State.Idle;
        }
    }

    private void OnDisable() {
        playerScanner.OnDetectedTarget.RemoveAllListeners();
        playerScanner.OnNotDetectedTarget.RemoveAllListeners();
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(transform, enemy.detectionAngle, enemy.viewDistance);
    }
#endif
}
