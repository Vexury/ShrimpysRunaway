using UnityEngine;

public class TopDownController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.2f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Features")]
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private bool useVariableJump = true;
    [SerializeField] private bool useCoyoteTime = true;
    [SerializeField] private bool useJumpBuffer = true;
    [SerializeField] private bool useRotation = true;

    private Vector2 moveInput;
    private bool isRunning;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool jumpHeld;
    private bool jumpRequestedThisFrame;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        inputReader.EnableGameplayInput();
    }

    private void OnEnable()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.SprintEvent += OnRun;
        inputReader.JumpEvent += OnJump;
    }

    private void OnDisable()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.SprintEvent -= OnRun;
        inputReader.JumpEvent -= OnJump;
    }

    private void Update()
    {
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        HandleMovement();
        HandleRotation();
        jumpRequestedThisFrame = false;
    }

    private void UpdateCoyoteTime()
    {
        if (characterController.isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;
    }

    private void UpdateJumpBuffer()
    {
        jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        bool canJump = useCoyoteTime ? coyoteTimer > 0f : characterController.isGrounded;
        bool jumpReady = useJumpBuffer ? jumpBufferTimer > 0f : jumpRequestedThisFrame;

        if (jumpReady && canJump)
        {
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        float appliedGravity = gravity;
        if (verticalVelocity < 0f)
            appliedGravity *= fallGravityMultiplier;
        else if (useVariableJump && !jumpHeld && verticalVelocity > 0f)
            appliedGravity *= lowJumpMultiplier;

        verticalVelocity += appliedGravity * Time.deltaTime;

        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetHorizontal = new Vector3(moveInput.x, 0f, moveInput.y) * targetSpeed;

        if (useAcceleration)
        {
            float rate = targetHorizontal.magnitude > 0.01f ? acceleration : deceleration;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetHorizontal, rate * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = targetHorizontal;
        }

        characterController.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!useRotation || horizontalVelocity.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnMove(Vector2 input) => moveInput = input;
    private void OnRun(bool isPressed) => isRunning = isPressed;
    private void OnJump(bool isPressed)
    {
        jumpHeld = isPressed;
        if (isPressed)
        {
            jumpBufferTimer = jumpBufferTime;
            jumpRequestedThisFrame = true;
        }
    }
}
