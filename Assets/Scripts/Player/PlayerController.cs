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
    
    private void Awake() {
        //init input system
        inputs = new InputAssets();
        
        // subscribe active input
        inputs.PlayerControl.Move.performed += GetDirection;
        inputs.PlayerControl.Move.canceled += GetDirection;
        inputs.PlayerControl.StartTouch.performed += ShowJoystick;
        inputs.PlayerControl.HoldTouch.canceled += HideJoystick;

        //get component
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");

    }

    private void OnEnable() {
        inputs.PlayerControl.Enable();
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
        Move();
        RotationLook();
        HandleGravity();
        HandlAnimation();
    }


    private void Move() {
        Vector3 motionMove = dirMove * speed * Time.deltaTime;
        Vector3 motionFall = Vector3.up * fallingVelocity * Time.deltaTime;
        controller.Move(motionMove + motionFall);
    }

    private void RotationLook() {
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
        joystickRectTrans.position = ctx.ReadValue<Vector2>();
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
            ObstacleDamageable damageable = hit.transform.GetComponentInParent<ObstacleDamageable>();
            damageable.TakeDamge(hit.point, 5f);
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

    private void OnDisable() {
        inputs.PlayerControl.Disable();
        // unsubscribe active input
        inputs.PlayerControl.Move.performed -= GetDirection;
        inputs.PlayerControl.Move.canceled -= GetDirection;
        inputs.PlayerControl.StartTouch.performed -= ShowJoystick;
        inputs.PlayerControl.HoldTouch.canceled -= HideJoystick;
    }
}
