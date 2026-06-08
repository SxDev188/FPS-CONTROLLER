using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private CinemachineInputAxisController cameraInputAxisController;
    
    [SerializeField]
    private CinemachineCamera cinemachineCamera;

    [SerializeField]
    private Transform playerStandingPosition;
    
    [SerializeField]
    private Transform playerCrouchPosition;

    [SerializeField]
    private CapsuleCollider standingCollider;

    [SerializeField]
    private CapsuleCollider crouchingCollider;

    [Header("Move Values")]
    [SerializeField]
    private float moveForce = 1.5f; 

    [SerializeField]
    private float jumpForce = 5.0f;

    [Space, SerializeField]
    private bool useTogglableSprint;

    [SerializeField]
    private bool useTogglableCrouch;

    [Space, SerializeField]
    private float sprintMultiplier = 2f;

    [SerializeField]
    private float slideSpeedMultiplier = 1.2f;
    
    [Header("Raycast Settings")]
    [SerializeField]
    private float groundCheckLength = 1.1f;

    [Header("Control Values")]
    [SerializeField]
    private float fallOffTime = 1f;

    [SerializeField]
    private float airControl = 0.3f;

    [SerializeField]
    private float groundedAirControl = 1f;

    [Header("Sensitivity")]
    [SerializeField]
    private float mouseSensitivty = 1f;

    [SerializeField]
    private float mouseAcceleration = 0f;

    [SerializeField]
    private float mouseDecceleration = 0f;

    [Header("Debug")]
    [SerializeField]
    private bool toggleDebug;

    // ==========================================//
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    private Rigidbody playerRigidBody;
    private Camera playerCamera;

    private Vector3 finalMoveVector;
    private bool shouldJump;
    private float jumpTimer = 0;
    private bool grounded;
    private bool shouldSprint;
    private bool shouldCrouch;
    private bool shouldSlide;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");

        playerCamera = Camera.main;
        playerRigidBody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        finalMoveVector = transform.forward * moveValue.y + transform.right * moveValue.x;
        grounded = isGrounded();

        jumpTimer += Time.deltaTime;

        if (jumpAction.WasPressedThisFrame() && (grounded || jumpTimer < fallOffTime))
        {
            shouldJump = true;
        }

        if (!useTogglableSprint)
        {
            if (sprintAction.IsPressed())
                shouldSprint = true;
            else
                shouldSprint = false;
        }
        else
        {
            if (sprintAction.WasPressedThisFrame())
                shouldSprint = !shouldSprint;
        }


        foreach(var c in cameraInputAxisController.Controllers)
        {
            c.Driver.AccelTime = mouseAcceleration;
            c.Driver.DecelTime = mouseDecceleration;
            if(c.Name == "Look X (Pan)")
            {
                c.Input.Gain = mouseSensitivty;

            }
            if(c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = -mouseSensitivty;
            }
        }


        if (useTogglableCrouch)
        {
            if (crouchAction.WasPressedThisFrame())
            {
                shouldCrouch = !shouldCrouch;
            }
        }
        else
        {
            if(crouchAction.IsPressed())
                shouldCrouch = true;
            else
                shouldCrouch = false;
        }


        if (shouldCrouch)
        {
            cinemachineCamera.Follow = playerCrouchPosition;
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;

            if (shouldSprint)
            {
                shouldSlide = true;
            }
            else
            {
                shouldSlide = false;
            }
        }
        else
        {
            cinemachineCamera.Follow = playerStandingPosition;
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
            shouldSlide = false;
        }

    }

    private void FixedUpdate()
    {

        transform.eulerAngles = new Vector3(0, playerCamera.transform.eulerAngles.y, 0);

        Vector3 velocity = playerRigidBody.linearVelocity;
        Vector3 targetVelocity = finalMoveVector.normalized * moveForce;
        targetVelocity *= !shouldSprint ? 1 : sprintMultiplier;
        targetVelocity *= !shouldSlide ? 1 : slideSpeedMultiplier;
        Vector3 appliedVelocity = new Vector3(targetVelocity.x - velocity.x, 0, targetVelocity.z - velocity.z);

        float moveControlMultiplier = grounded ? groundedAirControl : airControl;

        playerRigidBody.AddForce(appliedVelocity * moveControlMultiplier, ForceMode.VelocityChange);

        if (shouldJump)
        {
            playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            shouldJump = false;
        }
    }

    bool isGrounded()
    {
        RaycastHit hit;
        int layerMask = ~(1 << 3);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckLength, layerMask))
        {
            if(toggleDebug)
                Debug.DrawRay(transform.position, -hit.normal * hit.distance, Color.red); 

            jumpTimer = 0;
            return true;
        }
        else
        {
            if(toggleDebug)
                Debug.DrawRay(transform.position, Vector3.down * groundCheckLength, Color.white); 
            
            return false;
        }

    }
}
