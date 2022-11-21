using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //public
    public float speed = 10f;
    public RectTransform joystickRectTrans; 
    public float gravity = -9.81f;
    //private
    private InputAssets inputs;
    private CharacterController controller;
    private Vector3 dirMove;
    private float fallingVelocity;
    private Animator animator;
    private int velocityHash;
    private bool isStart, isPause, isAttack;
    private GameManager gameManager;
    private PlayerAttack playerAttack;
    
    private void Awake() {
        //init input system
        inputs = new InputAssets();
        
        // subscribe active input

        //get component
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        velocityHash = Animator.StringToHash("Velocity");

        gameManager = GameManager.Instance;

    }

    private void OnEnable() {
        inputs.PlayerControl.Enable();
        inputs.PlayerControl.Move.performed += GetDirection;
        inputs.PlayerControl.Move.canceled += GetDirection;
        inputs.PlayerControl.StartTouch.performed += ShowJoystick;
        inputs.PlayerControl.HoldTouch.canceled += HideJoystick;

        gameManager.OnStart.AddListener(OnStartGame);
        gameManager.OnPause.AddListener(OnPauseGame);
        gameManager.OnResume.AddListener(OnResumeGame);
        gameManager.OnEndGame.AddListener(OnEndGame);

        playerAttack.OnAttack += OnAttack;

    }
    // Start is called before the first frame update
    void Start()
    {
        //hide joystick out of UI view
        joystickRectTrans.position = new Vector2(9999999, 9999999);
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart) {
            Move();
            RotationLook();
        }
        HandleGravity();
        HandlAnimation();
    }


    private void Move() {
        Vector3 motionMove = dirMove * speed * Time.deltaTime;
        Vector3 motionFall = Vector3.up * fallingVelocity * Time.deltaTime;
        controller.Move(motionMove + motionFall);
    }

    private void RotationLook() {
        if(isAttack) return;
        if(dirMove != Vector3.zero) {
            Quaternion rotLook = Quaternion.LookRotation(dirMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotLook, 20f * Time.deltaTime);
        }
    }

    private void GetDirection(InputAction.CallbackContext ctx) {
        Vector2 dir = ctx.ReadValue<Vector2>();
        dirMove = new Vector3(dir.x, 0, dir.y);
    }

    private void ShowJoystick(InputAction.CallbackContext ctx) {
        if(!isPause && isStart) {
            joystickRectTrans.position = ctx.ReadValue<Vector2>();
        }
    }

    private void HideJoystick(InputAction.CallbackContext ctx) {
        //hide joystick out of UI view
        joystickRectTrans.position = new Vector2(9999999, 9999999);
    }
    
    private void HandleGravity() {
        if(controller.isGrounded) {
            fallingVelocity = gravity/10;
        } else {
            fallingVelocity += gravity/10;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            IDamageable obstacleDamageable = hit.transform.GetComponentInParent<IDamageable>();
            Vector3 dir = hit.transform.position -  transform.position;
            dir.y = 0;
            obstacleDamageable.TakeDamge(hit.point, dir.normalized * 10 );
        }
    }

    private void HandlAnimation() {
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float Velocity = horizontalVelocity.magnitude/speed;
        if(Velocity > 0) {
            animator.SetFloat(velocityHash, Velocity);
        } else {
            float v = animator.GetFloat(velocityHash);
            v = v> 0.01f ? Mathf.Lerp(v, 0, 20f * Time.deltaTime): 0;
            animator.SetFloat(velocityHash, v);
        }
    }

    private void OnStartGame() {
        isStart = true;
    }
    private void OnPauseGame() {
        isPause = true;
    }

    private void OnResumeGame() {
        isPause = false;
    }

    private void OnEndGame(bool isWin) {
        isPause = true;
    }

    private void OnAttack(bool attack) {
        isAttack = attack;
    }

    private void OnDisable() {
        inputs.PlayerControl.Disable();
        // unsubscribe active input
        inputs.PlayerControl.Move.performed -= GetDirection;
        inputs.PlayerControl.Move.canceled -= GetDirection;
        inputs.PlayerControl.StartTouch.performed -= ShowJoystick;
        inputs.PlayerControl.HoldTouch.canceled -= HideJoystick;

        gameManager.OnStart.RemoveListener(OnStartGame);
        gameManager.OnPause.RemoveListener(OnPauseGame);
        gameManager.OnResume.RemoveListener(OnResumeGame);
        gameManager.OnEndGame.RemoveListener(OnEndGame);

        playerAttack.OnAttack -= OnAttack;


    }
}
