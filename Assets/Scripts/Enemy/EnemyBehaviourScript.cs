using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{   
    [SerializeField] private Enemy enemy;
    public AttackAction attackAction;

    private enum State {
        Idle,
        Patrol,
        Chase,
        Attack
    }
    public enum TypePatrol {
        MoveAround,
        standInPlace
    }
    public TypePatrol typePatrol;
    [HideInInspector] public Vector3[] patrolList;
    [HideInInspector] public Vector3 standPos;
    [SerializeField] private PlayerScanner playerScanner = new PlayerScanner();
    private NavMeshAgent agent;
    private GameObject FieldOfView;
    private int patrolIndex = 0;
    private Vector3 playerPosition;
    private Transform player;
    private State state, prevState;
    private float IdleTimer,speedRotation = 10;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemy.speed;
        agent.angularSpeed = enemy.angularSpeed;
        agent.acceleration = enemy.acceleration;

    }

    private void OnEnable() {

        playerScanner.OnDetectedTarget.AddListener(HandleChangeStateWhenDetected);
        playerScanner.OnNotDetectedTarget.AddListener(HandleChangeStateWhenNotDetected);
        
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
                Patrol();
                break;
            case State.Attack:
                prevState = state;
                IdleTimer = 0;
                Attack(playerPosition);
                break;
            case State.Chase:
                prevState = state;
                IdleTimer = 0;
                Chase(playerPosition);
                break;
            default:
                break;
        }
    }

    private void OnDisable() {
        playerScanner.OnDetectedTarget.RemoveAllListeners();
        playerScanner.OnNotDetectedTarget.RemoveAllListeners();
    }

    private void Idle() {
        agent.SetDestination(transform.position);
        IdleTimer += Time.deltaTime;
        if(IdleTimer >= enemy.IdleTime) {
            state = State.Patrol;
            IdleTimer = 0;
        }
    }

    private void Patrol() {
        Vector3 walkPoint = patrolList[patrolIndex];
        switch(typePatrol) {
            case TypePatrol.MoveAround:
                if(agent.remainingDistance <= agent.stoppingDistance) {
                    IdleTimer += Time.deltaTime;
                    if(IdleTimer > enemy.IdleTime) {
                        patrolIndex++;
                        if(patrolIndex >= patrolList.Length) {
                            patrolIndex = 0;
                        }
                        agent.SetDestination(walkPoint);
                        IdleTimer = 0;
                    }
                }
                break;
            case TypePatrol.standInPlace:
                    IdleTimer += Time.deltaTime;
                    agent.SetDestination(standPos);
                    if(agent.remainingDistance <= agent.stoppingDistance) {
                        transform.rotation = LerpRotation(walkPoint, transform.position, speedRotation);
                        if(IdleTimer > enemy.IdleTime) {
                            patrolIndex++;
                            if(patrolIndex >= patrolList.Length) {
                                patrolIndex = 0;
                            }
                            IdleTimer = 0;
                        }
                    }
                break;
            default:
                break;
        }
    }

    private void Attack(Vector3 playerPos) {
        agent.SetDestination(transform.position);
        Vector3 dirLook = playerPos - transform.position;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, speedRotation * Time.deltaTime);
        if(Mathf.Abs(Quaternion.Angle(transform.rotation, rotLook)) <= 20) {
            attackAction.Attack(player);
        }
    }

    private void Chase(Vector3 playerPos) {
        agent.SetDestination(playerPos);
        if(agent.remainingDistance != 0 && agent.remainingDistance <= agent.stoppingDistance) {
            state = State.Idle;
        }
    }


    private Quaternion LerpRotation(Vector3 pos1, Vector3 pos2, float speed) {
        Vector3 dirLook = pos1 - pos2;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        return Quaternion.Lerp(transform.rotation, rotLook, speed * Time.deltaTime);
    }
    
    public void HandleChangeStateWhenDetected(Transform transform) {
        player = transform;
        playerPosition = player.position;
        state = State.Attack;
    }

    public void HandleChangeStateWhenNotDetected() {
        if(prevState == State.Attack) {
            state = State.Chase;
        }
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(transform, enemy.detectionAngle, enemy.viewDistance);
    }
#endif
}
