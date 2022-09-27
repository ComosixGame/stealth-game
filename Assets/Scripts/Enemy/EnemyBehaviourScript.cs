using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{   
    [SerializeField] private Enemy enemy;
    public EnemyWeapon enemyWeapon;
    public Transform detector;

    private enum State {
        Idle,
        Patrol,
        Chase,
        Attack,
        Alert
    }
    public enum TypePatrol {
        MoveAround,
        StandInPlace
    }
    public TypePatrol typePatrol;
    public Vector3[] patrolList;
    [HideInInspector] public Vector3 standPos;
    [SerializeField] private Scanner playerScanner = new Scanner();
    private NavMeshAgent agent;
    private GameObject FieldOfView;
    private int patrolIndex = 0;
    private Vector3 playerPosition;
    private Transform player;
    private State state, prevState;
    private float IdleTimer, alertTimer;
    private bool isDeadBody;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemy.speed;
        agent.angularSpeed = enemy.angularSpeed;
        agent.acceleration = enemy.acceleration;
    }

    private void OnEnable() {

        playerScanner.OnDetectedTarget.AddListener(HandleChangeStateWhenDetected);
        playerScanner.OnNotDetectedTarget.AddListener(HandleChangeStateWhenNotDetected);
        playerScanner.OnDetectedSubTarget.AddListener(HandleChangeStateWhenDetectedSubtarget);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //creata field of view
        FieldOfView = playerScanner.CreataFieldOfView(detector, detector.position, enemy.detectionAngle, enemy.viewDistance);
    }

    // Update is called once per frame
    void Update()
    {
        playerScanner.Scan(detector);
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
            case State.Alert:
                Alert();
                break;
            default:
                break;
        }
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
            case TypePatrol.StandInPlace:
                    IdleTimer += Time.deltaTime;
                    agent.SetDestination(standPos);
                    if(agent.remainingDistance <= agent.stoppingDistance) {
                        transform.rotation = LerpRotation(walkPoint, transform.position, enemy.speedRotation);
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, enemy.speedRotation * Time.deltaTime);
        if(Mathf.Abs(Quaternion.Angle(transform.rotation, rotLook)) <= 20) {
            enemyWeapon.Attack(player);
        }
    }

    private void Chase(Vector3 pos) {
        agent.SetDestination(pos);
        if(agent.remainingDistance != 0 && agent.remainingDistance <= agent.stoppingDistance) {
            if(!isDeadBody) {
                state = State.Idle;
            } else {
                state = State.Alert;
            }
        }
    }

    private void Alert() {
        if(alertTimer <= enemy.alertTime) {
            alertTimer += Time.deltaTime;
            if(agent.remainingDistance <= agent.stoppingDistance) {
                IdleTimer += Time.deltaTime;
                if(IdleTimer >= 0.5f) {
                    Vector3 pos = RandomNavmeshLocation(agent.height * 2);
                    agent.SetDestination(pos);
                    IdleTimer = 0;
                }
            }
        } else {
            alertTimer = 0;
            state = State.Idle;
        }
    }


    private Quaternion LerpRotation(Vector3 pos1, Vector3 pos2, float speed) {
        Vector3 dirLook = pos1 - pos2;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        return Quaternion.Lerp(transform.rotation, rotLook, speed * Time.deltaTime);
    }
    
    public void HandleChangeStateWhenDetected(List<RaycastHit> hitList) {
        player = playerScanner.DetectSingleTarget(hitList);
        playerPosition = player.position;
        state = State.Attack;
    }

    public void HandleChangeStateWhenNotDetected() {
        if(prevState == State.Attack) {
            state = State.Chase;
        }
    }

    private void HandleChangeStateWhenDetectedSubtarget(Transform _transform) {
        playerPosition =  _transform.position;
        isDeadBody = true;
        Destroy(_transform.GetComponent<Collider>());
        state = State.Chase;
    }

    private Vector3 RandomNavmeshLocation(float radius) {
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;     
        }
        return finalPosition;
     }

    private void OnDisable() {
        playerScanner.OnDetectedTarget.RemoveListener(HandleChangeStateWhenDetected);
        playerScanner.OnNotDetectedTarget.RemoveListener(HandleChangeStateWhenNotDetected);
        playerScanner.OnDetectedSubTarget.RemoveListener(HandleChangeStateWhenDetectedSubtarget);
    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        playerScanner.EditorGizmo(detector, enemy.detectionAngle, enemy.viewDistance);
    }
#endif
}
