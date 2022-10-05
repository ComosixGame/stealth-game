using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{   
    [SerializeField] private Enemy enemy;
    public Transform rootScanner;
    public Rig aimLayer;
    public GameObject weapon;
    public GameObject bullet;
    [SerializeField] private WeaponHolder weaponHolder;
    private Weapon enemyWeapon;
    private enum State {
        Idle,
        Patrol,
        Chase,
        Attack,
        Looking
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
    private float IdleTimer;
    private bool isDeadBody;
    private bool Alerted;
    private Animator animator;
    private int velocityHash;
    private GameManager gameManager;
    private EnemyDamageable enemyDamageable;
    private bool readyAttack;
    private void Awake() {
        gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = enemy.angularSpeed;
        agent.acceleration = enemy.acceleration;

        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");

        GameObject w = weaponHolder.AddWeapon(weapon);

        enemyWeapon = w.GetComponent<Weapon>();
        enemyWeapon.getAnimationWeaponPlay(animator);

        enemyDamageable = GetComponent<EnemyDamageable>();
        enemyDamageable.OnTakeDamge.AddListener(HandleWhenTakeDamge);

        enemyDamageable.setHealth(enemy.health);
    }

    private void OnEnable() {

        gameManager.OnEnemyAlert.AddListener(HandleOnAlert);
        gameManager.OnEnemyAlertOff.AddListener(HandleOnAlertOff);

        playerScanner.OnDetectedTarget.AddListener(HandleWhenDetected);
        playerScanner.OnNotDetectedTarget.AddListener(HandleWhenNotDetected);
        playerScanner.OnDetectedSubTarget.AddListener(HandleWhenDetectedSubtarget);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //creata field of view
        FieldOfView = playerScanner.CreataFieldOfView(rootScanner, rootScanner.position, enemy.detectionAngle, enemy.viewDistance);

        // sửa lỗi khi stoppingDistance enemy không thể chuyển trạng thái sau khi chase
        if(agent.stoppingDistance == 0) {
            agent.stoppingDistance = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerScanner.Scan(rootScanner);
        HandleAnimation();
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
                Attack(playerPosition);
                break;
            case State.Chase:
                prevState = state;
                IdleTimer = 0;
                Chase(playerPosition);
                Debug.DrawLine(transform.position, playerPosition);
                break;
            case State.Looking:
                Looking();
                break;
            default:
                break;
        }

        if(state == State.Patrol) {
            agent.speed = enemy.speedPatrol;
        } else {
            agent.speed = enemy.speed;
        }
    }

    private void Idle() {
        IdleTimer += Time.deltaTime;
        agent.SetDestination(transform.position);
        if(!isDeadBody) {
            if(IdleTimer >= enemy.IdleTime) {
                state = State.Patrol;
                IdleTimer = 0;
            }
        } else {
            if(IdleTimer >= 0.5) {
                gameManager.EnemyTriggerAlert(playerPosition, enemy.alertTime);
                IdleTimer = 0;
            }
        }
    }

    private void Patrol() {
        if(patrolList != null && patrolList.Length > 0) {
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
                        agent.SetDestination(standPos);
                        if(agent.remainingDistance <= agent.stoppingDistance) {
                            IdleTimer += Time.deltaTime;
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
    }

    private void Attack(Vector3 playerPos) {
        agent.SetDestination(transform.position);
        Vector3 dirLook = playerPos - transform.position;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, enemy.speedRotation * Time.deltaTime);
        if(Mathf.Abs(Quaternion.Angle(transform.rotation, rotLook)) <= 20) {
            aimLayer.weight = Mathf.Lerp(aimLayer.weight, 1.1f, 20f * Time.deltaTime);
            if(aimLayer.weight == 1 && !readyAttack) {
                StartCoroutine(WaitForReadyAttack());
            }
            if(readyAttack) {
                IdleTimer += Time.deltaTime;
                enemyWeapon.Attack(player, playerScanner.layerMaskTarget, "FromEnemy");
            }
        }

        if(!Alerted && IdleTimer >= 1.5f) {
            gameManager.EnemyTriggerAlert(playerPos, enemy.alertTime);
        }
    }

    private void Chase(Vector3 pos) {
        agent.SetDestination(pos);
        if(agent.remainingDistance != 0 && agent.remainingDistance <= agent.stoppingDistance) {
            if(Alerted) {
                state = State.Looking;
                return;
            }
            state = State.Idle;
        }
    }

    private void Looking() {
        if(agent.remainingDistance <= agent.stoppingDistance) {
            IdleTimer += Time.deltaTime;
            if(IdleTimer >= 0.3f) {
                Vector3 pos = RandomNavmeshLocation(agent.height * 2);
                agent.SetDestination(pos);
                IdleTimer = 0;
            }
        }
    }


    private Quaternion LerpRotation(Vector3 pos1, Vector3 pos2, float speed) {
        //tính nội suy phép quay
        Vector3 dirLook = pos1 - pos2;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        rotLook.x = 0;
        rotLook.z = 0;
        return Quaternion.Lerp(transform.rotation, rotLook, speed * Time.deltaTime);
    }
    
    public void HandleWhenDetected(List<RaycastHit> hitList) {
        player = playerScanner.DetectSingleTarget(hitList);
        playerPosition = player.position;
        state = State.Attack;
    }

    public void HandleWhenNotDetected() {
        aimLayer.weight = Mathf.Lerp(aimLayer.weight, -0.1f, 20f * Time.deltaTime);
        if(prevState == State.Attack) {
            state = State.Chase;
        }
    }

    private void HandleWhenDetectedSubtarget(Transform _transform) {
        bool isDetected = _transform.GetComponentInParent<DeadBody>().isDetected;
        if(!isDetected) {
            state = State.Chase;
            playerPosition =  _transform.position;
            _transform.GetComponentInParent<DeadBody>().isDetected = true;
            isDeadBody = true;
        }
    }

    private Vector3 RandomNavmeshLocation(float radius) {
        //tính random điểm có thể đi trên nav mesh
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomDirection = Random.insideUnitSphere * 5f;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;     
        }
        return finalPosition;
    }

    private void HandleOnAlert(Vector3 pos) {
        Alerted = true;
        playerPosition = pos;
        if(state != State.Attack) {
            state = State.Chase; 
        }
    }

    private void HandleOnAlertOff() {
        isDeadBody = false;
        Alerted = false;
        if(state != State.Attack) {
            state = State.Patrol;
        }
    }

    
    private void HandleAnimation() {
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float Velocity = horizontalVelocity.magnitude/enemy.speed;
        if(Velocity > 0) {
            animator.SetFloat(velocityHash, Velocity);
        } else {
            float v = animator.GetFloat(velocityHash);
            v = Mathf.Lerp(v, -0.1f, 20f * Time.deltaTime);
            animator.SetFloat(velocityHash, v);
        }
    }

    private void HandleWhenTakeDamge(Vector3 dir) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position - dir * 5f, out hit, agent.height * 2, 1)) {
            playerPosition = hit.position;
            state = State.Chase;   
        }
    }

    IEnumerator WaitForReadyAttack() {
        yield return new WaitForSeconds(0.1f);
        readyAttack = true;
    }

    private void OnDisable() {
        gameManager.OnEnemyAlert.RemoveListener(HandleOnAlert);
        gameManager.OnEnemyAlertOff.RemoveListener(HandleOnAlertOff);

        playerScanner.OnDetectedTarget.RemoveListener(HandleWhenDetected);
        playerScanner.OnNotDetectedTarget.RemoveListener(HandleWhenNotDetected);
        playerScanner.OnDetectedSubTarget.RemoveListener(HandleWhenDetectedSubtarget);

        enemyDamageable.OnTakeDamge.RemoveListener(HandleWhenTakeDamge);

    }

    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if(rootScanner != null && enemy != null) {
            playerScanner.EditorGizmo(rootScanner, enemy.detectionAngle, enemy.viewDistance);
        }
    }
#endif
}
